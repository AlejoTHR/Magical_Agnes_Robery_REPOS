using UnityEngine;
using UnityEngine.InputSystem;

public class FireMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private Vector2 _detectionSize;

    [Header("Effects & Feedback")]
    [SerializeField] private GameObject _breakEffectPrefab;
    [SerializeField] private float _effectDestroyDelay = 0.66f;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _startFallClip;   // Plays only on toggle ON

    private Movement plymov;
    private PlayerInput _input;
    private AudioSource _audioSource;
    private CapsuleCollider2D _col;

    public float CannonSpeed;
    public float CannonAcceleration;

    public Transform _detectTrasnform;
    private bool _isFireMagicToggled = false;
    private bool _wasGroundedLastFrame;

    void Start()
    {
        plymov = GetComponent<Movement>();
        _input = GetComponent<PlayerInput>();
        _audioSource = GetComponent<AudioSource>();
        _col = GetComponent<CapsuleCollider2D>();

        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f;

        _wasGroundedLastFrame = plymov._grounded;
    }

    void Update()
    {
        if (_input.actions["Fire"].WasPressedThisFrame())
        {
            if (!plymov._grounded || _isFireMagicToggled)
            {
                bool previousState = _isFireMagicToggled;
                _isFireMagicToggled = !_isFireMagicToggled;

                if (_isFireMagicToggled && !previousState)
                {
                    if (_startFallClip != null) _audioSource.PlayOneShot(_startFallClip);
                }
            }
        }

        if (plymov._grounded && !_wasGroundedLastFrame)
        {
            if (_isFireMagicToggled)
            {
                TriggerLandingEffect();
            }
            _isFireMagicToggled = false;
        }

        _wasGroundedLastFrame = plymov._grounded;
    }

    void FixedUpdate()
    {
        if (_isFireMagicToggled)
        {
            ApplyFireMagicStats();
            CheckForBreakables();
        }
        else
        {
            ResetStats();
        }
    }

    private void TriggerLandingEffect()
    {
        // Animation Prefab only when hitting the GROUND (No sound)
        if (_breakEffectPrefab != null)
        {
            GameObject effect = Instantiate(_breakEffectPrefab, _detectTrasnform.position, Quaternion.identity);
            Destroy(effect, _effectDestroyDelay);
        }
    }

    private void ApplyFireMagicStats()
    {
        _stats.MaxFallSpeed = CannonSpeed;
        _stats.FallAcceleration = CannonAcceleration;
        plymov.usingFireMagic = true;
        plymov.usingWindMagic = false;
    }

    private void ResetStats()
    {
        plymov.usingFireMagic = false;
        _stats.MaxFallSpeed = 40;
        _stats.FallAcceleration = 80;
    }

    private void CheckForBreakables()
    {
        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(_detectTrasnform.position, _detectionSize, 0f);

        foreach (var obj in hitObjects)
        {
            if (obj.CompareTag("Destroyable"))
            {
                // Animation Prefab only (No sound)
                if (_breakEffectPrefab != null)
                {
                    GameObject effect = Instantiate(_breakEffectPrefab, obj.transform.position, Quaternion.identity);
                    Destroy(effect, _effectDestroyDelay);
                }

                Destroy(obj.gameObject);

                plymov._rb.linearVelocity = new Vector2(plymov._rb.linearVelocity.x, -CannonSpeed);
                plymov._grounded = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_detectTrasnform == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_detectTrasnform.position, _detectionSize);
    }
}