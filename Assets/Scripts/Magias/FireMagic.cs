using UnityEngine;
using UnityEngine.InputSystem;

public class FireMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private float _detectionPadding = 0.2f; // How much "extra" reach the hitbox has

    private Movement plymov;
    private PlayerInput _input;
    private AudioSource _impactSource;
    private CapsuleCollider2D _col;

    public float CannonSpeed;
    public float CannonAcceleration;
    [SerializeField] private AudioClip _impactClip;

    void Start()
    {
        // Now these are guaranteed to exist
        plymov = GetComponent<Movement>();
        _input = GetComponent<PlayerInput>();
        _impactSource = GetComponent<AudioSource>();
        _col = GetComponent<CapsuleCollider2D>();

        // Ensure AudioSource is configured for 2D
        _impactSource.playOnAwake = false;
        _impactSource.spatialBlend = 0f; // 0 is 2D, 1 is 3D
    }


    void FixedUpdate()
    {
        if (plymov.usingFireMagic)
        {
            CheckForBreakables();
        }

        bool firePressed = _input.actions["Fire"].IsPressed();

        if (!plymov._grounded && firePressed)
        {
            _stats.MaxFallSpeed = CannonSpeed;
            _stats.FallAcceleration = CannonAcceleration;
            plymov.usingFireMagic = true;
            plymov.usingWindMagic = false;
        }
        else if (plymov._grounded)
        {
            plymov.usingFireMagic = false;
            _stats.MaxFallSpeed = 40;
            _stats.FallAcceleration = 80;
        }
    }

    private void CheckForBreakables()
    {
        // 1. Calculate the new size: slightly wider and taller than the collider
        Vector2 boxSize = new Vector2(_col.size.x + _detectionPadding, 0.6f);

        // 2. Offset the center so it reaches slightly further down
        Vector2 checkPos = (Vector2)transform.position + Vector2.down * ((_col.size.y / 2f) + (_detectionPadding / 2f));

        // 3. Use the expanded box
        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(checkPos, boxSize, 0f);

        foreach (var obj in hitObjects)
        {
            if (obj.CompareTag("Destroyable"))
            {
                if (_impactClip != null) _impactSource.PlayOneShot(_impactClip);
                Destroy(obj.gameObject);

                plymov._rb.linearVelocity = new Vector2(plymov._rb.linearVelocity.x, -CannonSpeed);
                plymov._grounded = false;
            }
        }
    }

    // Visual debugging so you can see the detection area in the Scene View
    private void OnDrawGizmosSelected()
    {
        if (_col == null) _col = GetComponent<CapsuleCollider2D>();
        if (_col == null) return;

        Gizmos.color = Color.red;
        Vector2 boxSize = new Vector2(_col.size.x + _detectionPadding, 0.6f);
        Vector2 checkPos = (Vector2)transform.position + Vector2.down * ((_col.size.y / 2f) + (_detectionPadding / 2f));
        Gizmos.DrawWireCube(checkPos, boxSize);
    }
}