using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilarManager : MonoBehaviour
{
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

        if (pilarsBuilt >= pilarsRequired)
        {
            ecoDuct.SetActive(true);
        }
    }
}
