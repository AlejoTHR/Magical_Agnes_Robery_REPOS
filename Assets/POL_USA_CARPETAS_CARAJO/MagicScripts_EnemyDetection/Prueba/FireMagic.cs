using UnityEngine;

public class FireMagic : MonoBehaviour
{

    [SerializeField] private ScriptableStats _stats;
    private Movement plymov = null;
    public Rigidbody2D agnes;
    public Transform MotherAgnes;
    public float CannonSpeed;
    public float CannonAcceleration;
    public GameObject hitBox;
    private GameObject clone2;

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
        if (plymov.isGrounded() == false)
        {
            if (Input.GetKey(KeyCode.I))
            {
                _stats.MaxFallSpeed = CannonSpeed;
                _stats.FallAcceleration = CannonAcceleration;
                if (plymov.usingFireMagic == false)
                {
                    clone2 = Instantiate(hitBox, MotherAgnes, false);                
                }
                plymov.usingFireMagic = true;
                plymov.usingWindMagic = false;
                clone2.transform.position = agnes.transform.position;
            }
        }
        else if (!Input.GetKey(KeyCode.I) || plymov.isGrounded())
        {
            _stats.MaxFallSpeed = 40;
            _stats.FallAcceleration = 80;
            plymov.usingFireMagic = false;
            Destroy(clone2);
        }
    }
}