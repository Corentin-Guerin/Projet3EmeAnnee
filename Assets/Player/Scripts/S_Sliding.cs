using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Sliding : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private Transform _orientation;
    [SerializeField]
    private Transform _playerObj;

    private Rigidbody rb;
    private S_PlayerMovement pm;

    [Header("Sliding")]
    [SerializeField]
    private float _maxSlideTime;

    [SerializeField]
    private float _slideForce;
    private float _slideTimer;

    [SerializeField]
    private float _slideYScale;

    private float _startYScale;

    [Header("Input")]
    public KeyCode _slideKey = KeyCode.LeftControl;
    private float _horizontalInput;
    private float _verticalInput;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<S_PlayerMovement>();

        _startYScale = _playerObj.localScale.y;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(_slideKey) && (_horizontalInput != 0 || _verticalInput != 0))
        {
            StartSlide();
        }

        if(Input.GetKeyUp(_slideKey) && pm._isSliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm._isSliding)
        {
            SlidingMovement();
        }
    }

        private void StartSlide()
    {
        if (pm._isGrounded == true)
        {
            pm._isSliding = true;
            _playerObj.localScale = new Vector3(_playerObj.localScale.x, _slideYScale, _playerObj.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            _slideTimer = _maxSlideTime;
        }
        else
        {
            pm._isSliding = false;
        }

        
    }

    private void SlidingMovement()
    {
        Vector3 _inputDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;


        /*if (!pm._isGrounded && Input.GetKeyDown(_slideKey))
        {
            //Coroutine svp
        }*/

        //sliding normal 
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(_inputDirection.normalized * _slideForce, ForceMode.Force);
            _slideTimer -= Time.deltaTime;
        }
        
        //sliding slope 
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(_inputDirection) * _slideForce, ForceMode.Force);
        }

        if (_slideTimer <= 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        pm._isSliding = false;

        _playerObj.localScale = new Vector3(_playerObj.localScale.x, _startYScale, _playerObj.localScale.z);
    }
}
