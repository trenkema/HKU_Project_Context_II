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

        EventSystemNew<int, int>.RaiseEvent(Event_Type.QUEST_ADD_AMOUNT, quest.questID, 1);

        if (pilarsBuilt >= pilarsRequired)
        {
            EventSystemNew<int>.RaiseEvent(Event_Type.QUEST_DONE, quest.questID);
        }
    }
}
