using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNearBoat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.PLAYER_NEAR_BOAT, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.PLAYER_NEAR_BOAT, false);
        }
    }
}
