using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NPC", order = 1)]
public class npcScriptableObject : ScriptableObject
{
    public string npcName;
    public Sprite npcSprite;
}