using UnityEngine;
using UnityEngine.InputSystem;

public class FireMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    private Movement plymov;
    private PlayerInput _input;
    private AudioSource _impactSource; // For the explosion only

    public float CannonSpeed;
    public float CannonAcceleration;
    [SerializeField] private AudioClip _impactClip;

    void Start()
    {
        plymov = GetComponent<Movement>();
        _input = GetComponent<PlayerInput>();
        _impactSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!plymov._grounded && _input.actions["Fire"].IsPressed())
        {
            _stats.MaxFallSpeed = CannonSpeed;
            _stats.FallAcceleration = CannonAcceleration;
            plymov.usingFireMagic = true;
            plymov.usingWindMagic = false;
        }
        else
        {
            plymov.usingFireMagic = false;
            _stats.MaxFallSpeed = 40;
            _stats.FallAcceleration = 80;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (plymov != null && plymov.usingFireMagic && collision.gameObject.CompareTag("Destroyable"))
        {
            if (_impactClip != null) _impactSource.PlayOneShot(_impactClip);
            Destroy(collision.gameObject);
        }
    }
}