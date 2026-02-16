using UnityEngine;

public class WindMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    public Rigidbody2D agnes;
    private Movement plymov = null;
    public float fallspeed;
    public float slowmo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plymov = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (plymov == null) return;
        if (Input.GetKey(KeyCode.Z))
        {
             _stats.MaxFallSpeed = fallspeed;
              plymov.usingWindMagic = true;
             _stats.MaxSpeed = slowmo;
            
        }
        else
        {
            _stats.MaxFallSpeed = 40;
            plymov.usingWindMagic = false;
            _stats.MaxSpeed = 14;
        }
    }
}
