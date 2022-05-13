using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilar : MonoBehaviour
{
    [SerializeField] Quest quest;

    [SerializeField] GameObject buildPilar;

    [SerializeField] float hitsRequired = 3;

    [SerializeField] Gradient buildGradient;

    Material material;

    float amountOfHits = 0;

    bool canHit = false;

    private void OnEnable()
    {
        EventSystemNew<Quest>.Subscribe(Event_Type.ACTIVATE_QUEST, CanHit);
    }

    private void OnDisable()
    {
        EventSystemNew<Quest>.Unsubscribe(Event_Type.ACTIVATE_QUEST, CanHit);
    }

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;

        ColorFromGradient(0f);
    }

    private void CanHit(Quest _quest)
    {
        if (quest == _quest)
        {
            canHit = true;
        }
    }

    public void Hit(float _amount)
    {
        if (canHit)
        {
            amountOfHits += _amount;

            float gradient = amountOfHits / hitsRequired;

            ColorFromGradient(gradient);

            if (amountOfHits >= hitsRequired)
            {
                buildPilar.SetActive(true);

                EventSystemNew<Quest, int>.RaiseEvent(Event_Type.QUEST_ADD_AMOUNT, quest, 1);

                gameObject.SetActive(false);
            }
        }
    }

    public void ColorFromGradient(float value)  // float between 0-1
    {
        material.color = buildGradient.Evaluate(value);
    }
}
