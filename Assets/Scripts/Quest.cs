using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Quest : MonoBehaviour
{
    public string questName;

    public string questObjective;

    public TextMeshProUGUI questObjectiveText;

    public string questDescription;

    public int questID;

    public int questCurrentAmount;
    public int questMaxAmount;

    private void Awake()
    {
        EventSystemNew<int, int>.Subscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmount);

        questObjectiveText.text = string.Format("[{0}/{1}] {2}", questCurrentAmount, questMaxAmount, questObjective);
    }

    private void OnDisable()
    {
        EventSystemNew<int, int>.Unsubscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmount);
    }

    private void AddAmount(int _questID, int _questAddAmount)
    {
        if (questID == _questID)
        {
            questCurrentAmount += _questAddAmount;

            questObjectiveText.text = string.Format("[{0}/{1}] {2}", questCurrentAmount, questMaxAmount, questObjective);

            if (questCurrentAmount >= questMaxAmount)
            {
                EventSystemNew<int>.RaiseEvent(Event_Type.QUEST_DONE, questID);
            }
        }
    }
}
