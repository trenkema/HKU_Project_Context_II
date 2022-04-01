using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestCompletion : MonoBehaviour
{
    [SerializeField] Animator questAnimator;

    [SerializeField] TextMeshProUGUI questNameText;

    private void OnEnable()
    {
        EventSystemNew<Quest>.Subscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);

        EventSystemNew.Subscribe(Event_Type.GAME_FINISHED, GameFinished);
    }

    private void OnDisable()
    {
        EventSystemNew<Quest>.Unsubscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);

        EventSystemNew.Unsubscribe(Event_Type.GAME_FINISHED, GameFinished);
    }

    private void QuestCompleted(Quest _quest)
    {
        questNameText.text = _quest.questName;

        questAnimator.SetTrigger("QuestCompleted");
    }

    private void GameFinished()
    {
        questAnimator.SetTrigger("GameFinished");
    }
}
