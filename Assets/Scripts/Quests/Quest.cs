using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class Quest : ScriptableObject
{
    public string questName;

    public string questDescription;

    public string questObjective;

    public int questCurrentAmount;

    public int questMaxAmount;

    public int questID;
}
