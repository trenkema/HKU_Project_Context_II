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

    public bool isActive = false;

    public bool isCompleted = false;

    private void Awake()
    {
        EventSystemNew<Quest, int>.Subscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmount);

        questObjectiveText.text = string.Format("[{0}/{1}] {2}", questCurrentAmount, questMaxAmount, questObjective);
    }

    private void OnDisable()
    {
        EventSystemNew<Quest, int>.Unsubscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmount);
    }

    private void AddAmount(Quest _quest, int _questAddAmount)
    {
        if (this == _quest)
        {
            questCurrentAmount += _questAddAmount;

            questObjectiveText.text = string.Format("[{0}/{1}] {2}", questCurrentAmount, questMaxAmount, questObjective);

            if (questCurrentAmount >= questMaxAmount)
            {
                EventSystemNew<Quest>.RaiseEvent(Event_Type.QUEST_COMPLETED, this);
            }
        }
    }
}
