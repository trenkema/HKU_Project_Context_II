using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Ink.Runtime;

public class NPC : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera npcZoomCamera;

    [SerializeField] Transform focusRootTransform;

    [SerializeField] npcScriptableObject npc;

    [SerializeField] GameObject talkText;

    public BasicInkExample inkManager;

    bool playerInTrigger = false;

    bool hasTalked = false;

    Transform player;

    private void FixedUpdate()
    {
        // Rotate Towards Player
        if (playerInTrigger && inkManager.npcTalking)
        {
            Quaternion lookRotation = Quaternion.LookRotation((player.position - transform.position).normalized);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(0f, lookRotation.eulerAngles.y, 0f)), 5f * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !inkManager.npcTalking)
        {
            npcZoomCamera.LookAt = focusRootTransform;
            npcZoomCamera.gameObject.SetActive(true);

            talkText.SetActive(true);

            inkManager.OnTriggerNPC(npc.npcName);

            playerInTrigger = true;

            player = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && inkManager.npcTalking && playerInTrigger && !hasTalked)
        {
            talkText.SetActive(false);

            hasTalked = true;
        }
        else if (other.CompareTag("Player") && !inkManager.npcTalking && playerInTrigger && hasTalked)
        {
            hasTalked = false;

            talkText.SetActive(true);

            npcZoomCamera.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !inkManager.npcTalking && !hasTalked)
        {
            npcZoomCamera.gameObject.SetActive(false);

            inkManager.npcTalking = false;

            talkText.SetActive(false);

            inkManager.OnTriggerNPC(string.Empty);

            playerInTrigger = false;
        }
    }
}
