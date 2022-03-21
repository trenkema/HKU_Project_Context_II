using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChopTreeController : MonoBehaviour
{
    [SerializeField] AudioClip swingSound;

    [SerializeField] AudioClip[] choppingSounds;

    [SerializeField] AudioSource choppingAudioSource;
    [SerializeField] AudioSource swingingAudioSource;

    [SerializeField] int minDamage, maxDamage;

    [SerializeField] CinemachineImpulseSource treeShake;

    [SerializeField] Transform hitArea;

    [SerializeField] GameObject vfxTreeHit;

    [SerializeField] float hitRange = 0.5f;

    public void AnimationEvent_OnHit()
    {
        Vector3 colliderSize = Vector3.one * hitRange;

        Collider[] colliderArray = Physics.OverlapBox(hitArea.position, colliderSize);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ITreeDamageable treeDamageable))
            {
                choppingAudioSource.PlayOneShot(choppingSounds[Random.Range(0, choppingSounds.Length)]);

                int damageAmount = Random.Range(minDamage, maxDamage);

                treeDamageable.Damage(damageAmount);

                treeShake.GenerateImpulse();

                Instantiate(vfxTreeHit, hitArea.position, Quaternion.identity);

                Debug.Log("Tree Hit");
            }
        }
    }

    public void AnimationEvent_OnSwing()
    {
        swingingAudioSource.PlayOneShot(swingSound);
    }
}
