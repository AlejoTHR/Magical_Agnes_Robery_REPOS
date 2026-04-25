using UnityEngine;
using UnityEngine.InputSystem;

public class FireMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    private Movement plymov;
    private PlayerInput _input;
    private AudioSource _impactSource;
    private CapsuleCollider2D _col;

    public float CannonSpeed;
    public float CannonAcceleration;
    [SerializeField] private AudioClip _impactClip;

    void Start()
    {
        plymov = GetComponent<Movement>();
        _input = GetComponent<PlayerInput>();
        _impactSource = GetComponent<AudioSource>();
        _col = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        // 1. Check if we are currently "Cannonballing" and hitting a destroyable object
        // We do this BEFORE the grounded check shuts us down.
        if (plymov.usingFireMagic)
        {
            CheckForBreakables();
        }

        // 2. State Management
        bool firePressed = _input.actions["Fire"].IsPressed();

        if (!plymov._grounded && firePressed)
        {
            _stats.MaxFallSpeed = CannonSpeed;
            _stats.FallAcceleration = CannonAcceleration;
            plymov.usingFireMagic = true;
            plymov.usingWindMagic = false;
        }
        // Only turn off Fire Magic if we are grounded and NOT hitting a breakable
        else if (plymov._grounded)
        {
            plymov.usingFireMagic = false;
            _stats.MaxFallSpeed = 40;
            _stats.FallAcceleration = 80;
        }
    }

    private void CheckForBreakables()
    {
        // Create a small detection area at the player's feet
        Vector2 checkPos = (Vector2)transform.position + Vector2.down * (_col.size.y / 2f);
        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(checkPos, new Vector2(_col.size.x, 0.5f), 0f);

        foreach (var obj in hitObjects)
        {
            if (obj.CompareTag("Destroyable"))
            {
                // Break it!
                if (_impactClip != null) _impactSource.PlayOneShot(_impactClip);
                Destroy(obj.gameObject);

                // RELAUNCH: This is the secret sauce. 
                // We keep the downward velocity high so you don't "pause" on the block.
                plymov._rb.linearVelocity = new Vector2(plymov._rb.linearVelocity.x, -CannonSpeed);

                // Force grounded to false for one frame to prevent the state flip
                plymov._grounded = false;
            }
        }
    }

    // Keep this as a backup for high-speed physics tunneling
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (plymov.usingFireMagic && collision.gameObject.CompareTag("Destroyable"))
        {
            if (_impactClip != null) _impactSource.PlayOneShot(_impactClip);
            Destroy(collision.gameObject);
            plymov._rb.linearVelocity = new Vector2(plymov._rb.linearVelocity.x, -CannonSpeed);
        }
    }
}