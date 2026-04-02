using UnityEngine;

public class AnimationController : Movement
{
    public Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _anim.SetBool("Ground", _grounded);
        _anim.SetBool("Water", usingWaterMagic);
        _anim.SetBool("Wind", usingWindMagic);
        _anim.SetBool("Fire", usingFireMagic);
        if (_grounded){
            if ((_rb.linearVelocity.x > 2f || _rb.linearVelocity.x < -2f))
            {
                _anim.SetBool("Walk", true);
            }
            else
            {
                _anim.SetBool("Walk", false);
            }
        }
        else
        {
            if (_rb.linearVelocity.y > 0f){
                _anim.SetBool("Jump", true);
            }else  {
                _anim.SetBool("Ground", true);
                _anim.SetBool("Jump", false);
            }
        }

    }
}
