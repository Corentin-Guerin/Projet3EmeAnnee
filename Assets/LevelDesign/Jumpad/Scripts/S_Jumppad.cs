using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Jumppad : MonoBehaviour
{
    [SerializeField]
    Rigidbody _player;
    [SerializeField]
    private float _addforce = 100f;

    void Start()
    {
        //_player = GetComponent<Rigidbody>();   
    }
    private void Jumpadbump()
    {
        _player.velocity = new Vector3(_player.velocity.x * (_addforce/2), _addforce, _player.velocity.z);

        _player.AddForce(transform.up, ForceMode.Impulse);
    }

    public void OnTriggerEnter()
    {
        Jumpadbump();
    }

}
