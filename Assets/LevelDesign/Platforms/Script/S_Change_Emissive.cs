using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Change_Emissive : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> _lumiere;

/*    [SerializeField]
    private GameObject _playerHitbox;*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TriggerEnter");
            for (int i = 0; i < _lumiere.Count; i++)
            {
                _lumiere[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(255, 0, 0, 1));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < _lumiere.Count; i++)
            {
                _lumiere[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(255, 255, 255, 1));
            }
        }
    }
}
