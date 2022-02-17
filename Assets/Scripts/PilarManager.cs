using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilarManager : MonoBehaviour
{
    [SerializeField] Quest quest;
    
    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.PILAR_BUILD, AddPilar);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.PILAR_BUILD, AddPilar);
    }

    private void AddPilar()
    {
        EventSystemNew<Quest, int>.RaiseEvent(Event_Type.QUEST_ADD_AMOUNT, quest, 1);
    }
}
