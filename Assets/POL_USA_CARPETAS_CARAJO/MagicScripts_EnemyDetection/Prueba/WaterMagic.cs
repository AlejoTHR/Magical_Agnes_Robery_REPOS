using UnityEngine;

public class WaterMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    private Movement plymov = null;
    public Rigidbody2D agnes;

    public Vector2 WaterDashDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plymov = GetComponent<Movement>();
    }


    // Update is called once per frame
    void Update()
    {
        WaterDash();
    }

    void WaterDash()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            plymov.SetFrameVelocity(WaterDashDirection);

            plymov.usingWaterMagic = true;
            plymov.usingFireMagic = false;
        }
        else if(!Input.GetKeyDown(KeyCode.F) && plymov.usingWindMagic == false)
        {
            plymov.usingWaterMagic = false;
        }

    }

}
