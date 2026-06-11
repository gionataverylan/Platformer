using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _data;

    private Rigidbody2D _rb;
    private PlayerInputHandler _input;

    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

    [SerializeField] private LayerMask _groundLayer;

    private bool _isGrounded;
    private bool _isJumping;
    private bool _isJumpCut;

    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    private float _gravityScale;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        _gravityScale = _rb.gravityScale;

        _rb.gravityScale = _data.GravityStrength / Physics2D.gravity.y;
        _gravityScale = _rb.gravityScale;
    }

    private void Update()
    {
        UpdateTimers();
        CheckGrounded();
        HandleJumpBuffer();
        HandleCoyoteTime();
        UpdateGravity();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void UpdateTimers()
    {
        _coyoteTimeCounter = Mathf.Max(0, _coyoteTimeCounter - Time.deltaTime);
        _jumpBufferCounter = Mathf.Max(0, _jumpBufferCounter - Time.deltaTime);
    }

    private void CheckGrounded()
    {
        bool wasGrounded = _isGrounded;

        _isGrounded = Physics2D.OverlapBox(
            _groundCheckPoint.position,
            _groundCheckSize,
            0,
            _groundLayer
        );

        if(!wasGrounded && _isGrounded)
        {
            OnLand();
        }
    }

    private void OnLand()
    {
        _isJumping = false;
        _isJumpCut = false;

        _coyoteTimeCounter = _data.coyoteTime;
    }

    private void HandleCoyoteTime()
    {
        if (_isGrounded)
        {
            _coyoteTimeCounter = _data.coyoteTime;
        }
    }

    private void HandleJumpBuffer()
    {
        if (_input.JumpPressed)
        {
            _jumpBufferCounter = _data.jumpBufferTime;
        }
    }

    private void HandleMovement()
    {
        float targetSpeed = _input.MoveInput.x * _data.runMaxSpeed;

        float accelRate;
        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            accelRate = 1f / _data.runAccelerationTime;
        }
        else
        {
            accelRate = 1f / _data.runDecelerationTime;
        }

        float newSpedX = Mathf.MoveTowards(
            _rb.linearVelocity.x,
            targetSpeed,
            accelRate * Time.fixedDeltaTime * _data.runMaxSpeed
        );

        _rb.linearVelocity = new Vector2(newSpedX, _rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if(_jumpBufferCounter > 0 && _coyoteTimeCounter > 0 && !_isJumping)
        {
            ExecuteJump();
        }

        if(_input.JumpReleased && _isJumping && !_isJumpCut)
        {
            if(_rb.linearVelocity.y > 0)
            {
                _isJumpCut = true;
                _rb.linearVelocity = new Vector2(
                    _rb.linearVelocity.x,
                    _rb.linearVelocity.y * 0.5f
                );
            }
        }
    }

    private void ExecuteJump()
    {
        _isJumping = true;
        _isJumpCut = false;

        _jumpBufferCounter = 0;
        _coyoteTimeCounter = 0;


        _rb.linearVelocity = new Vector2(
            _rb.linearVelocity.x,
            _data.JumpForce
        );    
    }

    private void UpdateGravity()
    {
        if (_isJumpCut)
        {
            _rb.gravityScale = _gravityScale * _data.jumpCutGravityMultiplier;
        }
        else if (_rb.linearVelocity.y < 0)
        {
            _rb.gravityScale = _gravityScale * _data.fallGravityMultiplier;
        }
        else
        {
            _rb.gravityScale = _gravityScale;
        }
    }
}