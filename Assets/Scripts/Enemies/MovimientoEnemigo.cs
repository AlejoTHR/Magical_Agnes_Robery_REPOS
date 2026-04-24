using UnityEngine;

public class MovimientoEnemigo : MonoBehaviour
{
    #region Public Variables
    [Header("Mode Toggle")]
    public bool isCameraMode = false;
    public bool rotateSpriteWithCone = true;

    [Header("Movement Settings (Patrol)")]
    public float speed = 2f;
    public float waitTime = 1.0f;
    public Transform pointA;
    public Transform pointB;

    [Header("Camera Mode Settings")]
    public float startAngle = 45f;
    public float endAngle = 135f;
    public float rotationSpeed = 0.5f;
    public float pauseTime = 1.5f;
    public bool rotateClockwise = true;
    #endregion

    #region Private Variables
    private EnemyScript enemyScript;
    private Transform currentTarget;
    private float waitTimer;
    private bool isWaiting = false;
    private float lerpFactor = 0f;
    private bool movingForward = true;
    private SpriteRenderer sr;
    private Animator animator;
    #endregion

    public void Start()
    {
        enemyScript = GetComponent<EnemyScript>();
        sr = GetComponent<SpriteRenderer>(); // Initialize SpriteRenderer
        animator = GetComponent<Animator>(); // INITIALIZE ANMTOR CONTROLLER
        if (!isCameraMode)
        {
            if (pointB != null) currentTarget = pointB;
            UpdateFacing();
        }
    }

    void Update()
    {
        if (isCameraMode)
        {
            HandleCameraRotation();
        }
        else
        {
            HandlePatrol();
        }

        if (rotateSpriteWithCone && enemyScript != null)
        {
            SyncSpriteRotation();
        }
    }

    void HandleCameraRotation()
    {
        if (enemyScript == null) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0) isWaiting = false;
            return;
        }

        float step = rotationSpeed * Time.deltaTime;
        lerpFactor += movingForward ? step : -step;

        float actualStart = rotateClockwise ? startAngle : endAngle;
        float actualEnd = rotateClockwise ? endAngle : startAngle;

        float currentAngle = Mathf.Lerp(actualStart, actualEnd, lerpFactor);
        enemyScript.fovRotation = currentAngle;

        if (lerpFactor >= 1f) { lerpFactor = 1f; StartWait(pauseTime); movingForward = false; }
        else if (lerpFactor <= 0f) { lerpFactor = 0f; StartWait(pauseTime); movingForward = true; }
    }

    void SyncSpriteRotation()
    {
        float correctedRotation = -enemyScript.fovRotation - 110f;
        transform.eulerAngles = new Vector3(0, 0, correctedRotation);
    }

    void StartWait(float time)
    {
        isWaiting = true;
        waitTimer = time;
    }

    void HandlePatrol()
    {
        if (currentTarget == null) return;
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                animator.SetBool("IsMoving", true); // ANIMATION SETTER

                isWaiting = false;
                currentTarget = (currentTarget == pointB) ? pointA : pointB;
                UpdateFacing();
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f) StartWait(waitTime);
        }
    }

    public void UpdateFacing()
    {
        if (currentTarget == null || enemyScript == null || sr == null) return;

        Vector2 dir = (currentTarget.position - transform.position).normalized;

        // Update the FOV logic
        enemyScript.fovRotation = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        // --- SPRITE FLIP LOGIC ---
        // If moving right (positive x), don't flip. If moving left (negative x), flip.
        // Note: Depending on your original sprite orientation, you might need to swap true/false.
        if (dir.x > 0.01f)
        {
            sr.flipX = false;
        }
        else if (dir.x < -0.01f)
        {
            sr.flipX = true;
        }
    }
}