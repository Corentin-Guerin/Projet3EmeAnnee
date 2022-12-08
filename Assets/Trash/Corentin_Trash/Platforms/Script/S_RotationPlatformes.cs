 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RotationPlatformes : MonoBehaviour
{
    [SerializeField]
    private GameObject _centerPlatforms;

    [SerializeField]
    private float _degre = 90f;
    private float _alpha = 0.0f;
    public float _alphaSpeed = 0.001f;
    private bool _isTrigger = false;
    private bool _startMoving = false;
    private Vector3 _initialRotation;

    void Start()
    {

    }

    void Update()
    {
        if (_startMoving == true)
        {
            //_alpha += Time.deltaTime;
            if (_alpha >= 1)
            {
                _startMoving = false;
            }
        }

        if (_isTrigger)
        {

            if (Input.GetButtonDown("LeftRotation") && _startMoving == false)
            {
                _initialRotation = _centerPlatforms.transform.eulerAngles;
                _alpha = 0f;
               
                _startMoving = true;
                StartCoroutine(MovePlatformsRight(_centerPlatforms));
            }

            if (Input.GetButtonDown("RightRotation") && _startMoving == false)
            {
                _initialRotation = _centerPlatforms.transform.eulerAngles;
                _alpha = 0f;

                _startMoving = true;
                StartCoroutine(MovePlatformsLeft(_centerPlatforms));

            }

        }

    }

    IEnumerator MovePlatformsRight(GameObject platforms)
    {
        while (_startMoving == true)
        {
            _centerPlatforms.transform.Rotate( Vector3.right * _degre * _alphaSpeed );
            _alpha += _alphaSpeed;
            yield return new WaitForSeconds(0.01f);
        }
        
    }
    IEnumerator MovePlatformsLeft(GameObject platforms)
    {
        while (_startMoving == true)
        {
            _centerPlatforms.transform.Rotate(Vector3.left * _degre * _alphaSpeed);
            _alpha += _alphaSpeed ;
            yield return new WaitForSeconds(0.01f);
        }

    }


    public void BoxTriggersEnter()
    {
        Debug.Log("Trigger");
        _isTrigger = true;
    }
    public void BoxTriggersExit()
    {
        Debug.Log("NON Trigger");
        _isTrigger = false;
    }

}
