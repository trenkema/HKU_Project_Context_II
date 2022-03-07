using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class NPC : MonoBehaviour
{
    [SerializeField] npcScriptableObject npc;

    public BasicInkExample inkManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inkManager.StartStoryFromNPC(npc.npcName);
        }
    }
}
