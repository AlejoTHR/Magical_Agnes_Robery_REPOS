using UnityEngine;
using UnityEngine.InputSystem;

public class WindMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    private PlayerInput _input;
    private Movement plymov;
    private FireMagic fireExtinguisher;
    private WaterMagic waterMagic;

    public float fallspeed;
    public float slowmo;

    private bool _wasFireActiveBeforeWind;

    void Start()
    {
        plymov = GetComponent<Movement>();
        fireExtinguisher = GetComponent<FireMagic>();
        _input = GetComponent<PlayerInput>();
        waterMagic = GetComponent<WaterMagic>();
    }

    void Update()
    {
        if (!plymov.usingWindMagic)
        {
            _wasFireActiveBeforeWind = fireExtinguisher.enabled;
        }

        if (!plymov._grounded && _input.actions["Wind"].IsPressed())
        {
            _stats.MaxFallSpeed = fallspeed;
            _stats.MaxSpeed = slowmo;

            fireExtinguisher.enabled = false;
            plymov.usingFireMagic = false;
            plymov.usingWindMagic = true;
            plymov._rb.linearVelocity = new Vector2(plymov._rb.linearVelocityX, 0);
        }
        else if(plymov.usingWindMagic && !_input.actions["Wind"].IsPressed()) 
        {
            StopGliding();
        }
    }

    void StopGliding()
    {
        if (plymov.usingWindMagic)
        {
            if (fireExtinguisher != null && _wasFireActiveBeforeWind)
            {
                fireExtinguisher.enabled = true;
            }

            plymov.usingWindMagic = false;
            _stats.MaxFallSpeed = 40;
            _stats.MaxSpeed = 14;
            waterMagic.DashUsed = false;
        }
    }
}