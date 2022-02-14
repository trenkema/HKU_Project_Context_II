using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [SerializeField] List<QuestLibrary> quests = new List<QuestLibrary>();

    int activeQuestID = 0;

    private void Awake()
    {
        EventSystemNew<int>.Subscribe(Event_Type.QUEST_DONE, QuestDone);
    }

    private void OnDisable()
    {
        EventSystemNew<int>.Unsubscribe(Event_Type.QUEST_DONE, QuestDone);
    }

    private void QuestDone(int _questID)
    {
        foreach (var quest in quests)
        {
            if (quest.quest.questID == _questID)
            {
                foreach (var questText in quest.questTexts)
                {
                    questText.SetActive(false);
                }

                foreach (var questCompletionObject in quest.questCompletionObjects)
                {
                    questCompletionObject.SetActive(true);
                }

                activeQuestID++;

                break;
            }
        }

        foreach (var quest in quests)
        {
            if (quest.quest.questID == activeQuestID)
            {
                EventSystemNew<int>.RaiseEvent(Event_Type.ENABLE_QUEST, activeQuestID);

                foreach (var questText in quest.questTexts)
                {
                    questText.SetActive(true);
                }

                break;
            }
        }
    }
}

[System.Serializable]
public class QuestLibrary
{
    public string questName;

    public Quest quest;

    public GameObject[] questTexts;

    public GameObject[] questCompletionObjects;
}
