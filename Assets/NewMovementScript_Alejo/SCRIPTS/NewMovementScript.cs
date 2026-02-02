using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovementScript : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement")]
    public float RunVelocity = 0;

    [Header("Jump")]
    public float JumpForce = 0;

    public float WindJumpForce = 0;

    private Vector3 JumpVector = Vector3.zero;

    [Header("Ground Check")]
    public Transform GroundCheckPosition;
    public Vector3 GroundCheckSize = new Vector3(0.5f, 0.05f, 0);
    public LayerMask GroundLayer;

    [Header("Gravity")]
    public float GravityScale = 0;
    public float MaxFallSpeed = 0;
    public float FallSpeedMultiplier = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            // KEY PRESSING || USES UNITY PHYSICS SYSTEM WITH RIGIDBODY 2D
        if(Keyboard.current.dKey.IsPressed())
        {       // RIGIDBODY VELOCITY ALTERED BY UNITYT PHYSICS SYSTEM
            rb.linearVelocity = new Vector3 (RunVelocity, rb.linearVelocity.y);
        }
        else if (Keyboard.current.aKey.IsPressed())
        {      
            rb.linearVelocity = new Vector3(-RunVelocity, rb.linearVelocity.y);
        }
        
        if(IsGrounded() == true)
        {
            if (Keyboard.current.spaceKey.IsPressed())
            {          // JUMPS USING ADDFORCE | ADDFOCRE USES UNITY PHYSICS SYSTEM
                Jump();
            }
            else if(Keyboard.current.xKey.IsPressed())
            {
                WindJump();
            }
        }
        Gravity();
    }

    private void Gravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = GravityScale*FallSpeedMultiplier; // Faster Fall
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -MaxFallSpeed));
        }
        else
        {
            rb.gravityScale = GravityScale;
        }

    }
    private void Jump()
    {
        JumpVector = new Vector3(rb.linearVelocity.x, JumpForce, 0);
        rb.AddForce(JumpVector);
    }
    private void WindJump()
    {
        JumpVector = new Vector3(rb.linearVelocity.x, WindJumpForce, 0);
        rb.AddForce(JumpVector);
    }



    public  bool IsGrounded()
    {   // OverlapBox returns an array of collidres | If the list has items in it, returns true
        if (Physics2D.OverlapBox(GroundCheckPosition.position, GroundCheckSize, 0, GroundLayer))
        {
            return true;
        }

        return false;
    }


    private void OnDrawGizmosSelected()
    {
       Gizmos.color = Color.red;
       Gizmos.DrawCube(GroundCheckPosition.position, GroundCheckSize);
    }


}

