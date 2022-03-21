using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterAction : MonoBehaviour
{
    [SerializeField] string triggerObjectTag;

    bool isInTrigger = false;

    public void ExecuteTrigger(System.Action action)
    {
        if (isInTrigger)
            action.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerObjectTag))
        {
            isInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerObjectTag))
        {
            isInTrigger = false;
        }
    }
}
