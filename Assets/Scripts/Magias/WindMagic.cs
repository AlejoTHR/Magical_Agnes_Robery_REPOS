using UnityEngine;

public class WindMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    public Rigidbody2D agnes;
    private Movement plymov = null;
    private FireMagic fireExtinguisher = null;
    public float fallspeed;
    public float slowmo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plymov = GetComponent<Movement>();
        fireExtinguisher = GetComponent<FireMagic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (plymov == null) return;
        if (!plymov._grounded)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                _stats.MaxFallSpeed = fallspeed;
                fireExtinguisher.enabled = false;
                plymov.usingFireMagic = false;
                plymov.usingWindMagic = true;
                _stats.MaxSpeed = slowmo;
            }
            else
            {
                _stats.MaxFallSpeed = 40;
                fireExtinguisher.enabled = true;
                plymov.usingWindMagic = false;
                _stats.MaxSpeed = 14;
            }
        }
        else
        {
            _stats.MaxFallSpeed = 40;
            fireExtinguisher.enabled = true;
            plymov.usingWindMagic = false;
            _stats.MaxSpeed = 14;
        }
    }
}
