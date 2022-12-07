using UnityEngine;
using UnityEngine.Events;

public class S_TriggerEnter : MonoBehaviour
{
    public UnityEvent _onTriggerEnter;

    public void OnTriggerEnter()
    {
        _onTriggerEnter.Invoke();

    }
}