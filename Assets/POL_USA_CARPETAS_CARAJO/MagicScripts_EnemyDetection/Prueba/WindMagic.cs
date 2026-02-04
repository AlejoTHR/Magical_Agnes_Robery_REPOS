using UnityEngine;

public class WindMagic : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    public Rigidbody2D agnes;
    private Movement plymov = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plymov = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (plymov == null) return;
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (_stats.MaxFallSpeed > 0)
            {
                _stats.MaxFallSpeed = 0;
                plymov.usingWindMagic = true;
            }
            else
            {
                _stats.MaxFallSpeed = 40;
                plymov.usingWindMagic = false;
            }
        }
    }
}
