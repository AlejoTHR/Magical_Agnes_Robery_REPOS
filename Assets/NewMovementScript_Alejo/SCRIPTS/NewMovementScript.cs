using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovementScript : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement")]
    public float HorizontalMovement;
    public float MoveSpeed = 0;

    [Header("Jump")]
    public float JumpForce = 0;

    [Header("Ground Check")]
    public Transform GroundCheckPosition;
    public Vector3 GroundCheckSize = new Vector3(0.5f, 0.05f, 0);
    public LayerMask GroundLayer;

    [Header("Gravity")]
    public float GravityScale = 0;
    public float MaxFallSpeed = 0;
    public float FallSpeedMultiplier = 0;


    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(HorizontalMovement * MoveSpeed, rb.linearVelocity.y);
        Gravity();
    }


    public void Move(InputAction.CallbackContext context)
    {
        HorizontalMovement = context.ReadValue<Vector2>().x;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            if (context.performed)
            {   // HOLD JUMP | HIGHER JUMP
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
            }
            else if (context.canceled)
            {   // RELEASE JUMP | START FALLING
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce * 0.5f);
            }
        }   
    }
    public void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = GravityScale * FallSpeedMultiplier; // Faster Fall
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -MaxFallSpeed));
        }
        else
        {
            rb.gravityScale = GravityScale;
        }

    }
    public bool IsGrounded()
    {   // OverlapBox returns an array of collidres | If the list has items in it, returns true
        if (Physics2D.OverlapBox(GroundCheckPosition.position, GroundCheckSize, 0, GroundLayer))
        {
            return true;
        }

        return false;
    }
    public void OnDrawGizmosSelected()
    {
       Gizmos.color = Color.red;
       Gizmos.DrawCube(GroundCheckPosition.position, GroundCheckSize);
    }


}

