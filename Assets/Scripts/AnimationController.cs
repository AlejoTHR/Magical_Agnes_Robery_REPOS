using UnityEngine;

public class AnimationController : Movement
{
    Animator _anim;
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _anim.SetBool("grounded", _grounded);
        _anim.SetBool("Water", usingWaterMagic);
        _anim.SetBool("Wind", usingWindMagic);
        _anim.SetBool("Fire", usingFireMagic);
        if (_grounded){
            if ((_rb.linearVelocity.x > 2f || _rb.linearVelocity.x < -2f))
            {
                _anim.SetBool("Moving", true);
            }
            else
            {
                _anim.SetBool("Moving", false);
            }
        }
        else
        {
            if (_rb.linearVelocity.y > 0f){
                _anim.SetBool("Jumping", true);
            }else  {
                _anim.SetBool("Falling", true);
                _anim.SetBool("Jumping", false);
            }
        }

    }
}
