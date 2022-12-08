using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


 [RequireComponent(typeof(Collider))]

public class S_TriggerBox : MonoBehaviour
{
    private UnityAction _onTriggered;

    public void AddEventToOnTriggered(UnityAction action)
    {
        _onTriggered += action;
    }

    private void OnTriggerEnter(Collider other)
    {
        _onTriggered.Invoke();

    }

    private void OnTriggerExit(Collider other)
    {
        _onTriggered.Invoke();
    }


}
