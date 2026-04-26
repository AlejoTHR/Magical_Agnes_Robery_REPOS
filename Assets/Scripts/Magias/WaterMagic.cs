using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WaterMagic : MonoBehaviour
{
    private PlayerInput _input;
    private Movement plymov;

    [Header("Dash Settings")]
    public float dashPower = 40f;
    public float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;

    public bool DashUsed = false;
    private float _cooldownTimer = 0f;

    void Start()
    {
        plymov = GetComponent<Movement>();
        _input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        // 1. Always tick the timer down
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        // 2. Dash triggers only if cooldown is finished
        if (_input.actions["Water"].WasPressedThisFrame() && _cooldownTimer <= 0)
        {
            // If we are on ground, or we are in air and haven't used the air-dash yet
            if (plymov.isGrounded() || !DashUsed)
            {
                StartCoroutine(DashRoutine());
            }
        }

        // 3. Reset DashUsed only when grounded AND cooldown is finished
        // This prevents the "ground spam" while keeping the air-dash limit
        if (plymov.isGrounded() && _cooldownTimer <= 0)
        {
            DashUsed = false;
        }
    }

    IEnumerator DashRoutine()
    {
        DashUsed = true;
        _cooldownTimer = dashCooldown; // Start cooldown immediately

        plymov.usingWaterMagic = true;
        plymov.PlayDashSound();

        float dir = plymov.ReturnDirection();
        if (Mathf.Approximately(dir, 0))
            dir = transform.localScale.x > 0 ? 1f : -1f;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            // Force velocity every physics frame to beat the Movement script
            plymov._rb.linearVelocity = new Vector2(dashPower * dir, 0f);
            plymov.SetFrameVelocity(new Vector2(dashPower * dir, 0f));

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        plymov.usingWaterMagic = false;
    }
}