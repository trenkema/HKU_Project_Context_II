using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class NPC : MonoBehaviour
{
    [SerializeField] npcScriptableObject npc;

    [SerializeField] GameObject talkText;

    public BasicInkExample inkManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !inkManager.npcTalking)
        {
            talkText.SetActive(true);

            inkManager.OnTriggerNPC(npc.npcName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !inkManager.npcTalking)
        {
            inkManager.npcTalking = false;

            talkText.SetActive(false);

            inkManager.OnTriggerNPC(string.Empty);
        }
    }
}
