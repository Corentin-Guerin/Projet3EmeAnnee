using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TriggerBoxManager : MonoBehaviour
{
    public S_TriggerBoxListing[] BoxList;

    private void Start()
    {
        foreach(var list in BoxList)
        {
            list.ApplyEvent();
        }
    }
}
