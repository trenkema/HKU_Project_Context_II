using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestCompletion : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] TextMeshProUGUI questNameText;

    private void OnEnable()
    {
        EventSystemNew<Quest>.Subscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);
    }

    private void OnDisable()
    {
        EventSystemNew<Quest>.Unsubscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);
    }

    private void QuestCompleted(Quest _quest)
    {
        questNameText.text = _quest.questName;

        animator.SetTrigger("QuestCompleted");
    }
}
