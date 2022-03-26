using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Ink.Runtime;
using UnityEngine.InputSystem;
using StarterAssets;

public class NPC : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] ThirdPersonController playerController;

    [SerializeField] float waitDelay = 0.15f;

    [SerializeField] CinemachineVirtualCamera playerCamera;

    //[SerializeField] CinemachineVirtualCamera npcZoomCamera;

    [SerializeField] Transform playerLookAt;

    [SerializeField] Transform playerRootTransform;

    [SerializeField] Transform focusRootTransform;

    [SerializeField] npcScriptableObject npc;

    [SerializeField] GameObject talkText;

    public BasicInkExample inkManager;

    bool playerInTrigger = false;

    bool hasTalked = false;
    bool zoom = false;

    [SerializeField] float switchSpeed = 5f;

    [SerializeField] Transform player;

    [SerializeField] Transform lookAtPlayerParent;
    [SerializeField] Transform lookAtNPCParent;

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
            //npcZoomCamera.LookAt = focusRootTransform;
            //npcZoomCamera.Priority = playerCamera.Priority + 1;

            talkText.SetActive(true);

            inkManager.OnTriggerNPC(npc.npcName);

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

            //npcZoomCamera.Priority = playerCamera.Priority - 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !inkManager.npcTalking && !hasTalked)
        {
            //npcZoomCamera.Priority = playerCamera.Priority - 1;

            inkManager.npcTalking = false;

            talkText.SetActive(false);

            inkManager.OnTriggerNPC(string.Empty);

            playerInTrigger = false;

            zoom = false;

            playerController.isInNPCRange = false;
        }
    }

    public void ZoomNPC(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            //npcZoomCamera.LookAt = focusRootTransform;
            //npcZoomCamera.Priority = playerCamera.Priority + 1;

            talkText.SetActive(true);

            inkManager.OnTriggerNPC(npc.npcName);

            playerInTrigger = true;
        }
    }

    public void UnNPC(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            //npcZoomCamera.Priority = playerCamera.Priority - 1;

            inkManager.npcTalking = false;

            talkText.SetActive(false);

            inkManager.OnTriggerNPC(string.Empty);
        }
    }
}
