using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public GameObject cableBroken;
    public GameObject cablePickup;
    public GameObject cableCollider;

    public bool isGrabbed = false;

    public bool Interact()
    {
        if (isGrabbed == false)
        {
            cableBroken.SetActive(false);
            cablePickup.SetActive(true);
            cableCollider.SetActive(true);

            isGrabbed = true;
            return true;
        }
            return false;
    }

}
