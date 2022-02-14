using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField] int questID;

    [SerializeField] LayerMask layersToCollide;

    [SerializeField] ParticleSystem particleEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EcoDuct"))
        {
            EventSystemNew<int, int>.RaiseEvent(Event_Type.QUEST_ADD_AMOUNT, questID, 1);
        }

        if (IsInLayerMask(other.gameObject, layersToCollide))
        {
            //Instantiate(particleEffect, other.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}
