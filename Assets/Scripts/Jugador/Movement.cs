using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class Movement : MonoBehaviour, IPlayerController
{
    [Header("Settings")]
    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Menu Reference")]
    [SerializeField] private MenuPausa _pauseMenu;

    [Header("Master Audio Clips")]
    [SerializeField] private AudioClip _walkClip;
    [SerializeField] private AudioClip _windGlideClip;
    [SerializeField] private AudioClip _fireCannonballClip;
    [SerializeField] private AudioClip _waterDashClip;
    [SerializeField][Range(0, 1)] private float _masterVolume = 0.5f;

    private PlayerInput _input;
    private AudioSource _audioSource;
    [HideInInspector] public Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
    private Quaternion noRotate = Quaternion.identity;

    [Header("Magic States")]
    public bool usingFireMagic = false;
    public bool usingWindMagic = false;
    public bool usingWaterMagic = false;
    public bool isHiding = false;

    [Header("Grounding")]
    public bool _grounded;
    private float _lastGroundedTime;
    [SerializeField] private float _groundedGracePeriod = 0.05f;

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    private bool _jumpToConsume;

    ///////
    private ParticleSystem _JumpParticle;
    public void ShowJumpParticle() { _JumpParticle.Play(); }
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        _input = GetComponent<PlayerInput>();
        _audioSource = GetComponent<AudioSource>();

        _JumpParticle = GetComponentInChildren<ParticleSystem>();

        _audioSource.playOnAwake = false;
        _audioSource.volume = _masterVolume;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        GatherInput();
        HandleMasterAudio();
        transform.rotation = noRotate;
    }

    private void FixedUpdate()
    {
        if (isHiding) { _rb.linearVelocity = Vector2.zero; return; }

        // 1. Sincronizamos la velocidad interna con la del motor físico
        _frameVelocity = _rb.linearVelocity;

        CheckCollisions();
        HandleJump();
        HandleDirection();
        HandleGravity();

        // 2. Aplicamos la velocidad calculada al Rigidbody
        _rb.linearVelocity = _frameVelocity;
    }
    public float groundedSize;
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Keep the cast slightly thinner than the player to avoid catching walls
        Vector2 castSize = new Vector2(_col.size.x * groundedSize, _col.size.y);

        RaycastHit2D hit = Physics2D.CapsuleCast(
            _col.bounds.center,
            castSize,
            _col.direction,
            0,
            Vector2.down,
            _stats.GrounderDistance,
            _groundLayer
        );

        // FIX: Only count as ground if the surface is facing UP (y > 0)
        // hit.normal.y > 0.7f means the surface is flatter than a 45-degree angle
        bool groundHit = hit.collider != null && hit.normal.y > 0.7f;

        if (groundHit)
        {
            if (!_grounded)
            {
                _grounded = true;
                GroundedChanged?.Invoke(true, Mathf.Abs(_rb.linearVelocity.y));
            }
            _lastGroundedTime = Time.time;
        }
        else if (_grounded && Time.time > _lastGroundedTime + _groundedGracePeriod)
        {
            _grounded = false;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    private void HandleJump()
    {
        if (_jumpToConsume && (_grounded || Time.time < _lastGroundedTime + 0.1f))
        {
            _frameVelocity.y = _stats.JumpPower;
            _jumpToConsume = false;
            Jumped?.Invoke();
            ShowJumpParticle();
        }
        _jumpToConsume = false;
    }

    private void HandleDirection()
    {
        float targetSpeed = _frameInput.Move.x * _stats.MaxSpeed;

        if (_frameInput.Move.x != 0)
        {
            // Aceleración activa al presionar botones
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, targetSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // FRENADO: Usamos GroundDeceleration para llevar la velocidad a 0
            float decel = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, decel * Time.fixedDeltaTime);
        }
    }

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            // Pequeña fuerza negativa constante para "pegar" al jugador al suelo y evitar el hovering
            _frameVelocity.y = -0.1f;
        }
        else
        {
            // Gravedad normal cayendo hacia MaxFallSpeed
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, _stats.FallAcceleration * Time.fixedDeltaTime);
        }
    }

    private void GatherInput()
    {
        // 1. Gather movement and jump
        _frameInput = new FrameInput
        {
            JumpDown = _input.actions["Jump"].WasPressedThisFrame(),
            Move = _input.actions["Move"].ReadValue<Vector2>()
        };

        if (_frameInput.JumpDown) _jumpToConsume = true;

        // 2. Handle Pause Action
        // Check if the "Pause" action was pressed this frame
        if (_input.actions["Pause"].WasPressedThisFrame())
        {
            if (_pauseMenu != null)
            {
                if (_pauseMenu.juegoPausado) _pauseMenu.Reanudar();
                else _pauseMenu.Pausar();
            }
        }
    }

    private void HandleMasterAudio()
    {
        AudioClip desiredLoop = null;

        // Prioritize magic sounds
        if (usingWindMagic) desiredLoop = _windGlideClip;
        else if (usingFireMagic && !_grounded) desiredLoop = _fireCannonballClip; // Only loop fire if NOT grounded
        else if (_grounded && Mathf.Abs(_rb.linearVelocity.x) > 0.5f && !isHiding) desiredLoop = _walkClip;

        if (desiredLoop != null)
        {
            if (_audioSource.clip != desiredLoop || !_audioSource.isPlaying)
            {
                _audioSource.clip = desiredLoop;
                _audioSource.loop = true;
                _audioSource.Play();
            }
        }
        else
        {
            // If no loop is desired (like the moment we hit the ground), STOP immediately
            if (_audioSource.isPlaying && _audioSource.loop)
            {
                _audioSource.Stop();
                _audioSource.clip = null; // Clear the clip to force a refresh next time
                _audioSource.loop = false;
            }
        }
    }

    public void PlayDashSound()
    {
        if (_waterDashClip != null) _audioSource.PlayOneShot(_waterDashClip, _masterVolume);
    }

    public void SetFrameVelocity(Vector2 velocity) => _frameVelocity = velocity;
    public float ReturnDirection() => Mathf.Sign(transform.localScale.x);
    public bool isGrounded() => _grounded;
}

public struct FrameInput { public bool JumpDown; public Vector2 Move; }
public interface IPlayerController
{
    event Action<bool, float> GroundedChanged;
    event Action Jumped;
    Vector2 FrameInput { get; }
}