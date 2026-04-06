/*
 * INSTRUCTIONS FOR USE:
 * 1. Attach this to the Camera GameObject.
 * 2. PIVOT: Ensure the Sprite Pivot is at the base of the camera (the wall mount).
 * 3. SYNC: If the camera is 90 degrees off (pointing at the floor while cone is at the wall),
 * this script adds a +90 correction to match the visual lens to the FOV mesh.
 */

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
    #endregion

    public void Start()
    {
        enemyScript = GetComponent<EnemyScript>();
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
        // Based on your image, the sprite is 90 degrees offset from the cone logic.
        // We subtract 90 to align the 'Lens' of your sprite with the FOV mesh.
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
            if (waitTimer <= 0) { isWaiting = false; currentTarget = (currentTarget == pointB) ? pointA : pointB; UpdateFacing(); }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f) StartWait(waitTime);
        }
    }

    public void UpdateFacing()
    {
        if (currentTarget == null || enemyScript == null) return;
        Vector2 dir = (currentTarget.position - transform.position).normalized;
        enemyScript.fovRotation = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
    }
}