using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Ink.Runtime;
using UnityEngine.InputSystem;
using StarterAssets;

public class NPC : MonoBehaviour
{
    public BasicInkExample inkManager;

    [SerializeField] Animator animator;

    [SerializeField] ThirdPersonController playerController;

    [SerializeField] CinemachineVirtualCamera playerCamera;

    [SerializeField] Transform playerLookAt;

    [SerializeField] Transform playerRootTransform;

    [SerializeField] Transform focusRootTransform;

    [SerializeField] npcScriptableObject npc;

    [SerializeField] GameObject talkText;

    [SerializeField] float switchSpeed = 5f;

    [SerializeField] Transform player;

    [SerializeField] Transform lookAtPlayerParent;
    [SerializeField] Transform lookAtNPCParent;

    bool playerInTrigger = false;

    bool hasTalked = false;
    bool zoom = false;

    private void Start()
    {
        talkText.SetActive(false);
    }

    private void Update()
    {
        if (!zoom)
        {
            playerLookAt.SetParent(lookAtPlayerParent, true);
            playerLookAt.position = Vector3.Lerp(playerLookAt.position, playerRootTransform.position, switchSpeed * Time.deltaTime);
        }
        else
        {
            playerLookAt.SetParent(lookAtNPCParent, true);
            playerLookAt.position = Vector3.Lerp(playerLookAt.position, focusRootTransform.position, switchSpeed * Time.deltaTime);
        }
    }

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
            talkText.SetActive(true);

            inkManager.OnTriggerNPC(npc.npcName, animator);

            playerInTrigger = true;

            player = other.transform;

            zoom = true;

            playerController.isInNPCRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && inkManager.npcTalking && playerInTrigger && !hasTalked)
        {
            animator.SetBool("isSpeaking", true);

            talkText.SetActive(false);

            hasTalked = true;
        }
        else if (other.CompareTag("Player") && !inkManager.npcTalking && playerInTrigger && hasTalked)
        {
            animator.SetBool("isSpeaking", false);

            hasTalked = false;

            talkText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !inkManager.npcTalking && !hasTalked)
        {
            inkManager.npcTalking = false;

            talkText.SetActive(false);

            inkManager.OnTriggerNPC(string.Empty, null);

            playerInTrigger = false;

            zoom = false;

            playerController.isInNPCRange = false;
        }
    }
}
