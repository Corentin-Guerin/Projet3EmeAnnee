using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask _whatIsWall;
    public LayerMask _whatIsGround;
    [SerializeField] private float _wallRunForce;
    [SerializeField] private float _wallClimbSpeed;
    [SerializeField] private float _wallJumpUpForce;
    [SerializeField] private float _wallJumpSideForce;

    [SerializeField] private float _maxWallRunTime;
    private float _wallRunTimer;
    private int _wallJumpsDone;
    [SerializeField] private Transform _lastWall;


    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode _upwardsRunKey = KeyCode.LeftShift;
    public KeyCode _downwardsRunKey = KeyCode.LeftControl;
    private bool _isUpwardsRunning;
    private bool _isDownwardsRunning;
    private float _horizontalInput;
    private float _verticalInput;

    [Header("Limitations")]
    [SerializeField] private int _allowedWallJumps = 1;

    [Header("Detection")]
    [SerializeField] private float _wallCheckDistance;
    [SerializeField] private float _minJumpHeight;
    private RaycastHit _leftWallHit;
    private RaycastHit _rightWallHit;
    public bool _isWallLeft;
    public bool _isWallRight;
    [SerializeField] private bool _isWallRemembered;
    [SerializeField] private float _angleValue;

    [Header("Exiting")]
    private bool _isExitingWall;
    [SerializeField] private float _exitWallTime;
    private float _exitWallTimer;

    [Header("Gravity")]
    public bool _isUsingGravity;
    [SerializeField] private float _gravityCounterForce;

    [Header("References")]
    public Transform _orientation;
    private S_PlayerMovement pm;
    private Rigidbody rb;



    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<S_PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm._isWallRunning)
        {
            WallRunningMovement();
        }
    }
    private void CheckForWall()
    {
        //Vector3 currentAnglesRight = _orientation.right; //original Right
        //Vector3 currentAnglesLeft = -_orientation.right; //original Left
        //Vector3 currentAnglesRight = -_orientation.forward+_orientation.right; //degueux mais fonctionne à peu près
        Vector3 currentAnglesLeftv2 = Quaternion.AngleAxis(-_angleValue, _orientation.up) * _orientation.forward;
        Vector3 currentAnglesRightv2 = Quaternion.AngleAxis(_angleValue, _orientation.up) * _orientation.forward;

        _isWallRight = Physics.Raycast(transform.position, currentAnglesRightv2, out _rightWallHit, _wallCheckDistance, _whatIsWall);
        _isWallLeft = Physics.Raycast(transform.position, currentAnglesLeftv2, out _leftWallHit, _wallCheckDistance, _whatIsWall);

        if ((_isWallLeft || _isWallRight) && NewWallHit())
        {
            _wallJumpsDone = 0;
            _wallRunTimer = _maxWallRunTime;
        }
    }

    private void RememberLastWall()
    {
        if (_isWallLeft)
        {
            _lastWall = _leftWallHit.transform;
        }

        if (_isWallRight)
        {
            _lastWall = _rightWallHit.transform;
        }
        //add reset _lastWall
    }

        private bool NewWallHit()
    {
        if (_lastWall == null) { 
            return true;
        }

        if (_isWallLeft && _leftWallHit.transform != _lastWall) { 
            return true;
        }

        else if (_isWallRight && _rightWallHit.transform != _lastWall){
            return true;
        }

        return false;
    }



    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, _minJumpHeight, _whatIsGround);
    }

    private void StateMachine()
    {
        // Getting inputs
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        _isUpwardsRunning = Input.GetKey(_upwardsRunKey);
        _isDownwardsRunning = Input.GetKey(_downwardsRunKey);
        //State 1 - WallRunning
        if ((_isWallLeft || _isWallRight) && _verticalInput > 0 && AboveGround() && !_isExitingWall)
        {
            //start wallrun
            if (!pm._isWallRunning)
            {
                StartWallRun();
            }

            if(_wallRunTimer > 0)
            {
                _wallRunTimer -= Time.deltaTime;
            }

            if(_wallRunTimer <= 0 && pm._isWallRunning)
            {
                _isExitingWall = true;
                _exitWallTimer = _exitWallTime;
            }
            // walljump
            if (Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }
        }
        //State 2 - Exiting
        else if (_isExitingWall)
        {
            if (pm._isWallRunning)
            {
                StopWallRun();
            }
            if(_exitWallTimer > 0)
            {
                _exitWallTimer -= Time.deltaTime;
            }

            if(_exitWallTimer <= 0)
            {
                _isExitingWall = false;
            }
        }
        //State 3 - None
        else
        {
            if (pm._isWallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        pm._isWallRunning = true;

        _wallRunTimer = _maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        _isWallRemembered = false;
    }


    private void WallRunningMovement()
    {
        rb.useGravity = _isUsingGravity;
        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 _wallNormal = _isWallRight ? _rightWallHit.normal : _leftWallHit.normal;
        Vector3 _wallForward = Vector3.Cross(_wallNormal, transform.up);

        if((_orientation.forward - _wallForward).magnitude > (_orientation.forward - -_wallForward).magnitude)
        {
            _wallForward = -_wallForward;
        }

        //forward force
        rb.AddForce(_wallForward * _wallRunForce, ForceMode.Force);

        //upwards/downwards force
        if (_isUpwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, _wallClimbSpeed, rb.velocity.z);
        }
        if (_isDownwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -_wallClimbSpeed, rb.velocity.z);
        }

        //push to wall force
        if (!_isExitingWall && !(_isWallLeft && _horizontalInput > 0) && !(_isWallRight && _horizontalInput < 0))
        {
            rb.AddForce(-_wallNormal * 100, ForceMode.Force);
        }

        if (_isUsingGravity)
        {
            rb.AddForce(transform.up * _gravityCounterForce, ForceMode.Force);
        }

        if (!_isWallRemembered)
        {
            RememberLastWall();
            _isWallRemembered = true;
        }
    }

    private void StopWallRun()
    {
        pm._isWallRunning = false;
    }
    
    //Reset _lastWall
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Ground")
        {
            _lastWall = null; 
        }
    }


    private void WallJump()
    {
        bool firstJump = true;
        //enter exiting wall state

        _isExitingWall = true;
        _exitWallTimer = _exitWallTime;

        Vector3 wallNormal = _isWallRight ? _rightWallHit.normal : _leftWallHit.normal;

        Vector3 forceToApply = transform.up * _wallJumpUpForce + wallNormal * _wallJumpSideForce;

        firstJump = _wallJumpsDone < _allowedWallJumps;
        _wallJumpsDone++;

        if (!firstJump)
            forceToApply = new Vector3(forceToApply.x, 0f, forceToApply.z);
        //add force

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        RememberLastWall();

        StopWallRun();
    }
}
