using UnityEngine;
/* * HOW TO USE:
 * 1. Attach to the Player GameObject (or the visual child with the Animator).
 * 2. Requires an Animator with Bool parameters: "Ground", "Water", "Wind", "Fire", "Jump", and "Walk".
 * 3. Automatically handles sprite flipping (scaling x by -1) based on Rigidbody velocity.
 */
public class AnimationController : MonoBehaviour
{
    private Animator _anim;
    private Movement _plymov;
    private Rigidbody2D _rb;


    // Track the current facing direction to avoid unnecessary scale updates
    private bool _facingRight = true;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _plymov = GetComponent<Movement>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_plymov == null) return;

        // --- Flipping Logic ---
        float moveInput = _rb.linearVelocity.x;

        if (moveInput > 0.1f && !_facingRight)
        {
            Flip();
        }
        else if (moveInput < -0.1f && _facingRight)
        {
            Flip();
        }

        // --- Animation Logic ---
        _anim.SetBool("Ground", _plymov.isGrounded());
        _anim.SetBool("Water", _plymov.usingWaterMagic);
        _anim.SetBool("Wind", _plymov.usingWindMagic);
        _anim.SetBool("Fire", _plymov.usingFireMagic);

        if (_plymov.isGrounded())
        {
            _anim.SetBool("Jump", false);
            bool isWalking = Mathf.Abs(_rb.linearVelocity.x) > 0.1f;
            _anim.SetBool("Walk", isWalking);
        }
        else
        {
            _anim.SetBool("Jump", _rb.linearVelocity.y > 0.1f);
            _anim.SetBool("Walk", false);
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing
        _facingRight = !_facingRight;

        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}