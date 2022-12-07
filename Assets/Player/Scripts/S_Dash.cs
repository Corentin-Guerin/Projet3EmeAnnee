using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Dash : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody _rb;
    private S_PlayerMovement _pm;

    [Header("Dashing")]
    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashUpwardForce;
    [SerializeField] private float _dashDuration;

    [Header("Settings")]
    [SerializeField] private bool _isUsingCameraForward = true;
    [SerializeField] private bool _isAllowingAllDirections = true;
    [SerializeField] private bool _isDisablingGravity = true;
    [SerializeField] private bool _isResettingVel = true;

    [Header("Cooldown")]
    [SerializeField] private float _dashCd;
    [SerializeField] private float _dashCdTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _pm = GetComponent<S_PlayerMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Dash"))
            DashFunction();

        if (_dashCdTimer > 0)
            _dashCdTimer -= Time.deltaTime;
    }

    private void DashFunction()
    {
        if (_dashCdTimer > 0) return;
        else _dashCdTimer = _dashCd;

        _pm._isDashing = true;

        Transform forwardT;

        if (_isUsingCameraForward)
            forwardT = playerCam;
        else
            forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);
        Vector3 forceToApply = direction * _dashForce + orientation.up * _dashUpwardForce;

        if (_isDisablingGravity)
               _rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), _dashDuration);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if (_isResettingVel)
        {
            _rb.velocity = Vector3.zero;
        }
        _rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        _pm._isDashing = false;

        if (_isDisablingGravity)
        {
            _rb.useGravity = true;
        }
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (_isAllowingAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }
        if (verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }
        return direction.normalized;
    }
            
}
