using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilarManager : MonoBehaviour
{
    [SerializeField] Quest quest;

    [SerializeField] int pilarsRequired;

    [SerializeField] GameObject ecoDuct;

    int pilarsBuilt;
    
    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.PILAR_BUILD, AddPilar);

        ecoDuct.SetActive(false);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.PILAR_BUILD, AddPilar);
    }

    private void AddPilar()
    {
        pilarsBuilt++;

        EventSystemNew<Quest, int>.RaiseEvent(Event_Type.QUEST_ADD_AMOUNT, quest, 1);

        if (pilarsBuilt >= pilarsRequired)
        {
            EventSystemNew<Quest>.RaiseEvent(Event_Type.QUEST_COMPLETED, quest);
        }
    }
}
