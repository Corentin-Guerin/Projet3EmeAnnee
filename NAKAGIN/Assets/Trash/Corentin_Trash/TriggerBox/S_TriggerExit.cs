using UnityEngine;
using UnityEngine.Events;

public class S_TriggerExit : MonoBehaviour
{
    public UnityEvent _onTriggerEnter;

    public void OnTriggerExit()
    {
        _onTriggerEnter.Invoke();

    }
}