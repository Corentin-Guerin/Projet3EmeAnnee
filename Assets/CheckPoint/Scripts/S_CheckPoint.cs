using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CheckPoint : MonoBehaviour
{
    [SerializeField] private Transform _respawnCapsule;
    [SerializeField] private Transform _respawnCoordonne;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("checkPoint");
             _respawnCoordonne.transform.position = _respawnCapsule.transform.position ;
             _respawnCoordonne.transform.position = new Vector3(_respawnCoordonne.transform.position.x,_respawnCoordonne.transform.position.y,_respawnCoordonne.transform.position.z);
            Physics.SyncTransforms();
        }
    }

}
