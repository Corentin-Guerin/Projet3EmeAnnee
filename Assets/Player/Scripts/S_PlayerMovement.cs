using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float _moveSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _slideSpeed;
    [SerializeField] private float _wallRunSpeed;
    [SerializeField] private float _climbUpSpeed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashSpeedChangeFactor;
    private float _desiredMoveSpeed;
    private float _lastDesiredMoveSpeed;
    private float _speedChangeFactor;
    
    private MovementState _lastState;
    private bool _isKeepingMomentum;

    private float _speedIncreaseMultiplier;
    private float _slopeIncreaseMultiplier;
    [SerializeField] private float _groundDrag;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    bool _readyToJump;

    [Header("Crouching")]
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _crouchYScale;
    private float _startYScale;

    [Header("Keybinds")]
    //public KeyCode _jumpKey = KeyCode.Space;
    public KeyCode _sprintKey = KeyCode.LeftShift;
    public KeyCode _crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _whatIsGround;
    public bool _isGrounded;

    [Header("Slope Handling")]
    [SerializeField] private float _maxSlopeAngle;
    private RaycastHit _slopeHit;
    private bool _exitingSlope;

    [Header("References")]
    [SerializeField] private S_Climbing ClimbingScript;

    [SerializeField] private Transform _orientation;

    float _horizontalInput;
    float _verticalInput;
    

    Vector3 _moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        crouching,
        sprinting,
        sliding,
        wallrunning,
        climbing,
        dashing,
        air
    }

    public bool _isSliding;
    public bool _isWallRunning;
    public bool _isClimbing;
    public bool _isDashing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        _readyToJump = true;

        _startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //Ground Check
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _whatIsGround);

        InputCommand();
        SpeedControl();
        StateHandler();
        

        //handle drag
        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
        {
            rb.drag = _groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovingPlayer();
    }

    private void InputCommand()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if (Input.GetButtonDown("Jump") && _readyToJump && _isGrounded)
        {
            _readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), _jumpCooldown);
        }
            //When crouch

        if (Input.GetKeyDown(_crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
            //add force cuz floating
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(_crouchKey))
        {
             transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //Mode - Dashing
        if (_isDashing)
        {
            state = MovementState.dashing;
            _desiredMoveSpeed = _dashSpeed;
            _speedChangeFactor = _dashSpeedChangeFactor;
        }
        //Mode - Climbing
        else if (_isClimbing)
        {
            state = MovementState.climbing;
            _desiredMoveSpeed = _climbUpSpeed;
        }

        //Mode - WallRunning
        else if (_isWallRunning)
        {
            state = MovementState.wallrunning;
            _desiredMoveSpeed = _wallRunSpeed;
        }

        //Mode - Slide
        else if (_isSliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                _desiredMoveSpeed = _slideSpeed;
            }
            else
            {
                _desiredMoveSpeed = _sprintSpeed;
            }
        }
        //Mode - Crouch 
        else if(Input.GetKey(_crouchKey))
        {
            state = MovementState.crouching;
            _desiredMoveSpeed = _crouchSpeed;
        }
        //Mode - Sprint
        else if (_isGrounded && Input.GetKey(_sprintKey) && !_isWallRunning)
        {
            state = MovementState.sprinting;
            _desiredMoveSpeed = _sprintSpeed;
        }
        //Mode - Walk
        else if (_isGrounded)
        {
            state = MovementState.walking;
            _desiredMoveSpeed = _walkSpeed;
        }
        //Mode - Air
        else
        {
            state = MovementState.air;
        }

        if (Mathf.Abs(_desiredMoveSpeed - _lastDesiredMoveSpeed) > 4f && _moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            _moveSpeed = _desiredMoveSpeed;
        }
        bool desiredMoveSpeedChanged = _desiredMoveSpeed != _lastDesiredMoveSpeed;
        if (_lastState == MovementState.dashing) _isKeepingMomentum = true;

        if (desiredMoveSpeedChanged)
        {
            if (_isKeepingMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                _moveSpeed = _desiredMoveSpeed;
            }
        }

        //_moveSpeed = _desiredMoveSpeed;
        _lastDesiredMoveSpeed = _desiredMoveSpeed;
        _lastState = state;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float _time = 0;
        float _difference = Mathf.Abs(_desiredMoveSpeed - _moveSpeed);
        float _startValue = _moveSpeed;
        float boostFactor = _speedChangeFactor;
        while (_time < _difference)
        {
            _moveSpeed = Mathf.Lerp(_startValue, _desiredMoveSpeed, _time / _difference);

            if (OnSlope())
            {
                float _slopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                float _slopeAngleIncrease = 1 + (_slopeAngle / 90f);

                _time += Time.deltaTime * _speedIncreaseMultiplier * _slopeIncreaseMultiplier * _slopeAngleIncrease;
            }
            else
                _time += Time.deltaTime * boostFactor;

                yield return null;
        }

        _moveSpeed = _desiredMoveSpeed;
        _speedChangeFactor = 1f;
        _isKeepingMomentum = false;
    }

    private void MovingPlayer()
    {
        if (state == MovementState.dashing) return;
        if (ClimbingScript._isExitingWall) return;
        //calculate movement direction 
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        //on slope
        if (OnSlope() && !_exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(_moveDirection) * _moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        else if (_isGrounded) 
        {
            rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }

        else if (!_isGrounded)
        {
            rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);
        }

        if (!_isWallRunning)
        {
            rb.useGravity = !OnSlope();
        }
    }

    private void SpeedControl()
    {
        //limiting speed on slope
        if (OnSlope() && !_exitingSlope) 
        {
            if(rb.velocity.magnitude > _moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * _moveSpeed;
            }
        }
        else
        {
            
            Vector3 _flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

             // limit velocity if needed
             if (_flatVelocity.magnitude > _moveSpeed)
             {
                    Vector3 _limitedVelocity = _flatVelocity.normalized * _moveSpeed;
                    rb.velocity = new Vector3(_limitedVelocity.x, rb.velocity.y, _limitedVelocity.z);
             }
        }
 
    }
    private void Jump()
    {
        _exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //rb.velocity = new Vector3(rb.velocity.x, ??, rb.velocity.z);

        rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;

        _exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f  + 0.3f))
        {
            float _angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return _angle < _maxSlopeAngle && _angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }
}
