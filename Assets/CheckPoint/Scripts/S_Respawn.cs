using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Respawn : MonoBehaviour
{
    
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _respawnplayer;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Respawn");
            _player.transform.position = _respawnplayer.transform.position;
            Physics.SyncTransforms();
        }
    }

}
