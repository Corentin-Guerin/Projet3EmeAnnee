using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MoveCamera : MonoBehaviour
{
    public Transform _cameraPosition;
 // Update is called once per frame
    void Update()
    {
        transform.position = _cameraPosition.position;
    }
}
