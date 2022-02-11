using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilar : MonoBehaviour
{
    [SerializeField] GameObject buildPilar;

    [SerializeField] int hitsRequired = 3;

    int amountOfHits = 0;

    public void Hit(int _amount)
    {
        amountOfHits += _amount;

        if (amountOfHits >= hitsRequired)
        {
            buildPilar.SetActive(true);

            EventSystemNew.RaiseEvent(Event_Type.PILAR_BUILD);

            gameObject.SetActive(false);
        }
    }
}
