using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class S_PlayerCam : MonoBehaviour
{
    public float _sensX;
    public float _sensY;

    [Header("References")]
    public S_PlayerMovement pm;
    public S_WallRunning wr;
    public Transform _orientation;
    public Transform player;

    float _xRotation;
    float _yRotation;
    float _mouseX;
    float _mouseY;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float _fov;
    [SerializeField] private float _wallRunFov;
    [SerializeField] private float _wallRunFovTime;
    [SerializeField] [Range(0, 30)] private float _camTiltWR;
    [SerializeField] [Range(0, 30)] private float _camTiltSlide;
    [SerializeField] private float _camTiltTime;
    [SerializeField] private float _wallSlideFovTime;
    [SerializeField] private float _wallSlideFov;


    public float tilt { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        CameraTiltWallRunFPS();
        CameraTiltSlide();
        // Mouse Input //
        _mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _sensX;
        _mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _sensY;
        ////////////////
        ///

        _yRotation += _mouseX;
        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, tilt);
        _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);

    }

    private void CameraTiltWallRunFPS()
    {
        if (pm._isWallRunning)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _wallRunFov, _wallRunFovTime * Time.deltaTime);
            if (wr._isWallLeft)
            {
                tilt = Mathf.Lerp(tilt, -_camTiltWR, _camTiltTime * Time.deltaTime);
            }

            else if (wr._isWallRight)
            {
                tilt = Mathf.Lerp(tilt, _camTiltWR, _camTiltTime * Time.deltaTime);
            }
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _fov, _wallRunFovTime * Time.deltaTime);
            tilt = Mathf.Lerp(tilt, 0, _camTiltTime * Time.deltaTime);
        }
    }
    private void CameraTiltSlide()
    {
        if (pm._isSliding)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _wallSlideFov, _wallSlideFovTime * Time.deltaTime);
                tilt = Mathf.Lerp(tilt, _camTiltSlide, _camTiltTime * Time.deltaTime);
            }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _fov, _wallSlideFovTime* Time.deltaTime);
            tilt = Mathf.Lerp(tilt, 0, _camTiltTime * Time.deltaTime);
        }
    }
}