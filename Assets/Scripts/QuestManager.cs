using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [SerializeField] List<QuestLibrary> quests = new List<QuestLibrary>();

    [SerializeField] Animator gameFinishedAnimator;

    [SerializeField] float delayForFadeOut = 5f;

    [SerializeField] float fadeOutDuration = 1f;

    [SerializeField] int amountOfQuests = 1;

    [Header("References")]
    [SerializeField] RectTransform activeQuestsContentRectTransform;
    [SerializeField] RectTransform completedQuestsContentRectTransform;

    [SerializeField] GameObject questPrefab;

    List<Quest> activeQuests = new List<Quest>();
    List<Quest> completedQuests = new List<Quest>();

    Dictionary<Quest, GameObject> questItems = new Dictionary<Quest, GameObject>();

    int activeQuestID = 0;

    private void Awake()
    {
        EventSystemNew<Quest>.Subscribe(Event_Type.ACTIVATE_QUEST, ActivateQuest);
        EventSystemNew<Quest>.Subscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);

        EventSystemNew<Quest, int>.Subscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmountToQuest);

        gameFinishedAnimator.SetBool("FadeIn", false);
        gameFinishedAnimator.SetBool("FadeOut", true);
    }

    private void OnDisable()
    {
        EventSystemNew<Quest>.Unsubscribe(Event_Type.ACTIVATE_QUEST, ActivateQuest);
        EventSystemNew<Quest>.Unsubscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);

        EventSystemNew<Quest, int>.Unsubscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmountToQuest);
    }

    private void ActivateQuest(Quest _quest)
    {
        if (!_quest.isActive && !_quest.isCompleted)
        {
            activeQuests.Add(_quest);

            GameObject newQuestPrefab = Instantiate(questPrefab);

            questItems.Add(_quest, newQuestPrefab);
            
            newQuestPrefab.transform.SetParent(activeQuestsContentRectTransform.transform, false);

            QuestItem questItem = newQuestPrefab.GetComponent<QuestItem>();

            questItem.questName.text = _quest.questName;
            questItem.questDescription.text = _quest.questDescription;

            questItem.UpdateAmount(_quest.questCurrentAmount, _quest.questMaxAmount);

            foreach (var quest in quests)
            {
                if (quest.quest.questID == _quest.questID)
                {
                    foreach (var questText in quest.questTexts)
                    {
                        questText.SetActive(true);
                    }

                    break;
                }
            }
        }
    }

    private void QuestCompleted(Quest _quest)
    {
        if (_quest.isActive && _quest.isCompleted)
        {
            _quest.isActive = false;
            _quest.isCompleted = true;

            activeQuests.Remove(_quest);
            completedQuests.Add(_quest);

            GameObject questObject = questItems[_quest].gameObject;
            QuestItem questItem = questObject.GetComponent<QuestItem>();

            questItem.transform.SetParent(completedQuestsContentRectTransform.transform, false);

            questItem.UpdateAmount(_quest.questMaxAmount, _quest.questMaxAmount);

            foreach (var quest in quests)
            {
                if (quest.quest.questID == _quest.questID)
                {
                    foreach (var questText in quest.questTexts)
                    {
                        questText.SetActive(false);
                    }

                    foreach (var questCompletionObject in quest.questCompletionObjects)
                    {
                        questCompletionObject.SetActive(true);
                    }

                    break;
                }
            }
        }
    }

    private void AddAmountToQuest(Quest _quest, int _amount)
    {
        GameObject questObject = questItems[_quest].gameObject;
        QuestItem questItem = questObject.GetComponent<QuestItem>();

        questItem.UpdateAmount(_quest.questCurrentAmount, _quest.questMaxAmount);
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
                    EventSystemNew<int>.RaiseEvent(Event_Type.ACTIVATE_QUEST, activeQuestID);

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
