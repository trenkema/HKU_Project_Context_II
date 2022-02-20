using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPlayerInteraction : MonoBehaviour
{
    bool isDisabled = false;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isDisabled)
        {
            if (other.CompareTag("Player"))
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isDisabled)
        {
            if (other.CompareTag("Player"))
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }

    public void DisableScript(bool _disable)
    {
        isDisabled = _disable;
    }
}
