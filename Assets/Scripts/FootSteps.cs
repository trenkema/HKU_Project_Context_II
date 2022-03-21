using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] AudioClip[] footStepSounds;

    [SerializeField] AudioSource audioSource;

    private void Step()
    {
        audioSource.PlayOneShot(footStepSounds[Random.Range(0, footStepSounds.Length)]);
    }
}
