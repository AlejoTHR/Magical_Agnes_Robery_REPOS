using UnityEngine;

public class FireMagic : MonoBehaviour
{

    [SerializeField] private ScriptableStats _stats;
    private Movement plymov = null;
    public Rigidbody2D agnes;
    public float CannonSpeed;
    public float CannonAcceleration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plymov = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        CannonBall();
    }

    void CannonBall()
    {
        if (Input.GetKey(KeyCode.V))
        {
            _stats.MaxFallSpeed = CannonSpeed;
            _stats.FallAcceleration = CannonAcceleration;
            plymov.usingFireMagic = true;
            plymov.usingWindMagic = false;
        }
        else if (!Input.GetKey(KeyCode.V) && plymov.usingWindMagic == false)
        {
            _stats.MaxFallSpeed = 40;
            _stats.FallAcceleration = 110;
        }

    }
}