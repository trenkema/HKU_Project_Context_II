using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [SerializeField] List<QuestLibrary> quests = new List<QuestLibrary>();

    [SerializeField] Animator gameFinishedAnimator;

    [SerializeField] float delayForFadeOut = 5f;

    [SerializeField] float fadeOutDuration = 1f;

    [SerializeField] int amountOfQuests = 1;

    int activeQuestID = 0;

    private void Awake()
    {
        EventSystemNew<int>.Subscribe(Event_Type.QUEST_DONE, QuestDone);

        gameFinishedAnimator.SetBool("FadeIn", false);
        gameFinishedAnimator.SetBool("FadeOut", true);
    }

    private void OnDisable()
    {
        EventSystemNew<int>.Unsubscribe(Event_Type.QUEST_DONE, QuestDone);
    }

    private void QuestDone(int _questID)
    {
        foreach (var quest in quests)
        {
            if (quest.quest.questID == _questID && activeQuestID == _questID)
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

        if (activeQuestID <= amountOfQuests)
        {
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
        else if (activeQuestID > amountOfQuests)
        {
            StartCoroutine(EndGame());
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(delayForFadeOut);

        gameFinishedAnimator.SetBool("FadeIn", true);
        gameFinishedAnimator.SetBool("FadeOut", false);

        yield return new WaitForSeconds(fadeOutDuration);

        SceneManager.LoadScene(0);
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
