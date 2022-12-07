using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Tirigger : MonoBehaviour
{
    [SerializeField] private Material myMaterial;

    private void OnTriggerEnter(Collider other)
    {
        myMaterial.color = Color.green;
    }

    private void OnTriggerExit(Collider other)
    {
        myMaterial.color = Color.red;
    }
}
