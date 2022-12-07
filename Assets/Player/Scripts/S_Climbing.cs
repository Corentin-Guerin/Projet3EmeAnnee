using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Climbing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientation;
    [SerializeField] private Rigidbody rb;
    public S_PlayerMovement pm;
    [SerializeField] private LayerMask _whatIsWall;

    [Header("Climbing")]
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _maxClimbTime;
    private float _climbTimer;
    private bool _isClimbing;

    [Header("ClimbJumping")]
    [SerializeField] private float _climbJumpUpForce;
    [SerializeField] private float _climbJumpBackForce;
    public KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private int _climbJumps;
    [SerializeField] private int _climbJumpsLeft;
    [SerializeField] private int _counterClimbPropulsion;

    [Header("Detection")]
    [SerializeField] private float _detectionLength;
    [SerializeField] private float _sphereCastRadius;
    [SerializeField] private float _maxWallLookAngle;
    private float _wallLookAngle;

    private RaycastHit _frontWallHit;
    private bool _isWallFront;

    private Transform _lastWall;
    private Vector3 _lastWallNormal;
    [SerializeField] private float _minWallNormalAngleChange;

    [Header("Exiting")]
    public bool _isExitingWall;
    [SerializeField] private float _exitWallTime;
    private float _exitingWallTimer;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (_isClimbing && !_isExitingWall)
        {
            ClimbingMovement();
        }

    }

    private void StateMachine()
    {
        //State 1 - Climbing
        if (_isWallFront && Input.GetKey(KeyCode.Z) && _wallLookAngle < _maxWallLookAngle && !_isExitingWall)
        {
            if (!_isClimbing && _climbTimer > 0)
            {
                StartClimbing();
            }

            if (_climbTimer > 0)
            {
                _climbTimer -= Time.deltaTime;
            }
            if (_climbTimer <= 0)
            {
                StopClimbingByTime();
            }
        }

        //State 2 - Exiting
        else if (_isExitingWall)
        {
            if (_isClimbing) StopClimbingByReachPoint();

            if (_exitingWallTimer > 0) _exitingWallTimer -= Time.deltaTime;
            if (_exitingWallTimer < 0) _isExitingWall = false;
        }

        //State 3 - None
        else
        {
            if (_isClimbing)
            {
                StopClimbingByReachPoint();
            }
        }

        if (_isWallFront && Input.GetKeyDown(jumpKey) && _climbJumpsLeft > 0)
        {
            ClimbJump();
        }
    }
    private void WallCheck()
    {
        _isWallFront = Physics.SphereCast(transform.position, _sphereCastRadius, _orientation.forward, out _frontWallHit, _detectionLength, _whatIsWall);
        _wallLookAngle = Vector3.Angle(_orientation.forward, -_frontWallHit.normal);

        bool newWall = _frontWallHit.transform != _lastWall || Mathf.Abs(Vector3.Angle(_lastWallNormal, _frontWallHit.normal)) > _minWallNormalAngleChange;

        if (_isWallFront && newWall || pm._isGrounded)
        {
            _climbTimer = _maxClimbTime;
            _climbJumpsLeft = _climbJumps;
        }
    }

    private void StartClimbing()
    {
        _isClimbing = true;
        pm._isClimbing = true;

        _lastWall = _frontWallHit.transform;
        _lastWallNormal = _frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, _climbSpeed, rb.velocity.z);
    }

    private void StopClimbingByReachPoint()
    {
        _isClimbing = false;
        pm._isClimbing = false;
        rb.AddForce(Vector3.down * _counterClimbPropulsion, ForceMode.Impulse);
    }
    private void StopClimbingByTime()
    {
        _isClimbing = false;
        pm._isClimbing = false;
    }

    private void ClimbJump()
    {
        _isExitingWall = true;
        _exitingWallTimer = _exitWallTime;
        Vector3 forceToApply = transform.up * _climbJumpUpForce + _frontWallHit.normal * _climbJumpBackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        _climbJumpsLeft--;
    }
}
