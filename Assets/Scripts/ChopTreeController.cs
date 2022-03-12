using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChopTreeController : MonoBehaviour
{
    [SerializeField] int minDamage, maxDamage;

    [SerializeField] CinemachineImpulseSource treeShake;

    [SerializeField] Transform hitArea;

    [SerializeField] GameObject vfxTreeHit;

    public void AnimationEvent_OnHit()
    {
        Vector3 colliderSize = Vector3.one * 0.3f;

        Collider[] colliderArray = Physics.OverlapBox(hitArea.position, colliderSize);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ITreeDamageable treeDamageable))
            {
                int damageAmount = Random.Range(minDamage, maxDamage);

                treeDamageable.Damage(damageAmount);

                treeShake.GenerateImpulse();

                Instantiate(vfxTreeHit, hitArea.position, Quaternion.identity);

                Debug.Log("Tree Hit");
            }
        }
    }
}
