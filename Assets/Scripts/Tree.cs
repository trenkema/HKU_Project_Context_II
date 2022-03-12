using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, ITreeDamageable
{
    [SerializeField] GameObject treeToDestroy;

    [SerializeField] int minHealth, maxHealth;

    [SerializeField] float treeLogUpOffset = 0.8f;

    [SerializeField] GameObject vfxTreeDestroyed;

    [SerializeField] GameObject treeLog;
    [SerializeField] GameObject treeStump;

    int health;

    private void Start()
    {
        health = Random.Range(minHealth, maxHealth);
    }

    public void Damage(int _amount)
    {
        health -= _amount;

        if (health <= 0)
        {
            OnDead();
        }
    }

    private void OnDead()
    {
        Instantiate(vfxTreeDestroyed, transform.position, transform.rotation);

        Vector3 treeLogOffset = Vector3.up * treeLogUpOffset;
        Instantiate(treeLog, transform.position + treeLogOffset, Quaternion.Euler(Random.Range(-1.5f, 1.5f), 0f, Random.Range(-1.5f, 1.5f)));

        Instantiate(treeStump, transform.position, transform.localRotation);

        Destroy(treeToDestroy);
    }
}
