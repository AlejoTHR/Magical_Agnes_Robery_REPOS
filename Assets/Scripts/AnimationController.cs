using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _anim;
    private Movement _plymov;
    private Rigidbody2D _rb;


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

        float moveInput = _rb.linearVelocity.x;

        if (moveInput > 0.1f && !_facingRight)
        {
            Flip();
        }
        else if (moveInput < -0.1f && _facingRight)
        {
            Flip();
        }

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
        _facingRight = !_facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}