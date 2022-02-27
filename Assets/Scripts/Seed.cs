using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField] Quest quest;

    [SerializeField] LayerMask layersToCollide;

    [SerializeField] GameObject[] vegetationPrefabs;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("EcoDuct"))
        {
            EventSystemNew<Quest, int>.RaiseEvent(Event_Type.QUEST_ADD_AMOUNT, quest, 1);

            int vegetationID = Random.Range(0, vegetationPrefabs.Length);

            GameObject vegetation = Instantiate(vegetationPrefabs[vegetationID], collision.contacts[0].point, Quaternion.identity);
        }

        if (IsInLayerMask(collision.transform.gameObject, layersToCollide))
        {
            Destroy(gameObject);
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}
