using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnBoat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.PLAYER_ON_BOAT, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.PLAYER_ON_BOAT, false);
        }
    }
}
