using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public struct S_TriggerBoxListing 
{
    public string _name;
    public UnityEvent _myEvent;
    public S_TriggerBox[] _boxUse;

    public void ApplyEvent()
    {
        foreach (var triggerBox in _boxUse)
        {
            triggerBox.AddEventToOnTriggered(CallEvent);
        }

    }

    private void CallEvent()
    {
        _myEvent.Invoke();
    }



}
