using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, ITreeDamageable
{
    [SerializeField] Quest quest;

    [SerializeField] GameObject treeToDestroy;

    [SerializeField] int minHealth, maxHealth;

    [SerializeField] float treeLogUpOffset = 0.8f;

    [SerializeField] GameObject vfxTreeDestroyed;

    [SerializeField] GameObject treeLog;
    [SerializeField] GameObject treeStump;

    int health;

    bool canChop = false;

    private void OnEnable()
    {
        EventSystemNew<Quest>.Subscribe(Event_Type.ACTIVATE_QUEST, CanChop);
    }

    private void OnDisable()
    {
        EventSystemNew<Quest>.Unsubscribe(Event_Type.ACTIVATE_QUEST, CanChop);
    }

    private void Start()
    {
        health = Random.Range(minHealth, maxHealth);
    }

    private void CanChop(Quest _quest)
    {
        if (quest == _quest)
        {
            canChop = true;
        }
    }

    public void Damage(int _amount)
    {
        if (canChop)
        {
            health -= _amount;

            if (health <= 0)
            {
                OnDead();
            }
        }
    }

    private void OnDead()
    {
        EventSystemNew<Quest, int>.RaiseEvent(Event_Type.QUEST_ADD_AMOUNT, quest, 1);

        Instantiate(vfxTreeDestroyed, transform.position, transform.rotation);

        Vector3 treeLogOffset = Vector3.up * treeLogUpOffset;
        Instantiate(treeLog, transform.position + treeLogOffset, Quaternion.Euler(Random.Range(-1.5f, 1.5f), 0f, Random.Range(-1.5f, 1.5f)));

        Instantiate(treeStump, transform.position, transform.localRotation);

        Destroy(treeToDestroy);
    }
}
