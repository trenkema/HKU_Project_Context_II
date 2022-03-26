using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeFixed : MonoBehaviour
{
    public GameObject cableFixed;
    public GameObject cablePickup;

    public Pipe pipeScript;

    public int pipeIndex = 0;

    public bool Interact()
    {
        if (pipeScript.isGrabbed == true)
        {
            EventSystemNew<int>.RaiseEvent(Event_Type.PIPE_FIXED, pipeIndex);

            cableFixed.SetActive(true);
            cablePickup.SetActive(false);

            gameObject.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
    }

}
