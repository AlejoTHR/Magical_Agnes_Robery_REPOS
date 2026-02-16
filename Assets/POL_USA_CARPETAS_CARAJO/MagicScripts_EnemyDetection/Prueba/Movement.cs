using System;
using UnityEngine;

//requires the character to have a rigidbody2d to execute
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Movement : MonoBehaviour, IPlayerController
{
    //declaration of variables
    //Refers to another script that manages the different variables used for the movement (gravity, jump height etc...)
    [SerializeField] private ScriptableStats _stats;
    //Declaration of the player rigidbody and collider
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    public bool isHiding = false;
    //Variables used to check for the input
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    private Quaternion noRotate = new Quaternion(0, 0, 0, 0);

    //state check
    public bool usingFireMagic = false;
    public bool usingWindMagic = false;
    public bool usingWaterMagic = false;

    #region Interface
    //Checks to see if an input was recieved, if the players grounded state changed and if he jumped
    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    #endregion
    //Variable used to check how much time has passed.
    private float _time;

    private void Awake()
    {
        //Collect the player objects rigidbody and collider
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        //Irrelevant and unnecessary to touch on. Collission stuff. 
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        //Makes the time go up based on the Time.deltaTime (aka not based on frames)
        _time += Time.deltaTime;
        //Gets the player input
        GatherInput();
        transform.rotation = noRotate;
    }

    private void GatherInput()
    {
        //the input collected on this exact frame
        _frameInput = new FrameInput
        {
            //Checks if the jump is pressed or held
            JumpDown = Input.GetButtonDown("Jump"),
            JumpHeld = Input.GetButton("Jump"),
            //Checks if the player moved
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };
        //Makes all Floats turn to ints when it comes to movement to make turns instant, etc. 
        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }
        //Checks if the player can Jump (this is about coyote time, dont touch this)
        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void FixedUpdate()
    {
        //Checks if the player hit something
        CheckCollisions();
        //Makes the player jump
        HandleJump();
        //Horizontal movement
        HandleDirection();
        //Vertical movement
        HandleGravity();
        //All together, applies it to the player. 
        ApplyMovement();
    }

    #region Collisions
    //Variables for checking WHEN the player left the ground and IF they are on the ground
    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;
    public bool isGrounded()
    {
        return _grounded;
    }

    public void SetFrameVelocity(Vector2 velocity)
    {
        _frameVelocity += velocity;
    }

    private float _facing = 1f;

    public float ReturnDirection()
    {
        if (_frameInput.Move.x != 0)
            _facing = Mathf.Sign(_frameInput.Move.x);

        return _facing;
    }

    //Self explanatory. Variables used here are declared below.
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping
    //Set of variables for the stuff above
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    //Again, jump buffering and coyote time are stuff that shouldnt be touched
    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        //If the jump button hasnt been held, stops upward momentum
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

        //Ignores jump buffering if over the coyote time window (took to long to jump or pressed it too early)
        if (!_jumpToConsume && !HasBufferedJump) return;

        //If neither of the above apply, do the jump 
        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        //restarts variables and gives the player velocity to jump
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        //If there is no input, decelerate
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        //If there is input, keep accelerating
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        //If you are on the ground, the gravity doesnt push you underground
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        //If you are on the air, increase gravity so you fall faster. 
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion
    //Turns the framevelocity (which we used to calculate the speed the player will have) as the linearVelocity (the speed the player will ACTUALLY have)
    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;

#if UNITY_EDITOR
    //error message
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;
    public Vector2 FrameInput { get; }
}
