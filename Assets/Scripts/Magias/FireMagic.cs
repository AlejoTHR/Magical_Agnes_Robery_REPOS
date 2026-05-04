using UnityEngine;
using UnityEngine.InputSystem;

public class FireMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private Vector2 _detectionSize;

    private Movement plymov;
    private PlayerInput _input;
    private AudioSource _impactSource;
    private CapsuleCollider2D _col;

    public float CannonSpeed;
    public float CannonAcceleration;
    [SerializeField] private AudioClip _impactClip;

    public Transform _detectTrasnform;

    // Track the toggle state internally
    private bool _isFireMagicToggled = false;

    void Start()
    {
        plymov = GetComponent<Movement>();
        _input = GetComponent<PlayerInput>();
        _impactSource = GetComponent<AudioSource>();
        _col = GetComponent<CapsuleCollider2D>();

        _impactSource.playOnAwake = false;
        _impactSource.spatialBlend = 0f;
    }

    void Update()
    {
        // Check for toggle input in Update for better responsiveness
        if (_input.actions["Fire"].WasPressedThisFrame())
        {
            // Only allow toggling ON if in the air; can always toggle OFF
            if (!plymov._grounded || _isFireMagicToggled)
            {
                _isFireMagicToggled = !_isFireMagicToggled;
            }
        }

        // Auto-disable toggle if we hit the ground
        if (plymov._grounded)
        {
            _isFireMagicToggled = false;
        }
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
        _stats.MaxFallSpeed = 40; // You might want to pull these from ScriptableStats defaults instead of hardcoding
        _stats.FallAcceleration = 80;
    }

    private void CheckForBreakables()
    {
        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(_detectTrasnform.position, _detectionSize, 0f);

        foreach (var obj in hitObjects)
        {
            if (obj.CompareTag("Destroyable"))
            {
                if (_impactClip != null) _impactSource.PlayOneShot(_impactClip);
                Destroy(obj.gameObject);

                // Note: If this is a downward "Cannon," we keep the downward velocity
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