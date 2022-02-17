using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<QuestLibrary> quests = new List<QuestLibrary>();
    [SerializeField] GameObject questPrefab;

    [SerializeField] GameObject questWindow;
    [SerializeField] GameObject forceQuestWindow;

    [SerializeField] GameObject noQuestsInGameText;

    [Header("Texts")]

    [SerializeField] TextMeshProUGUI activeTabText;
    [SerializeField] TextMeshProUGUI doneTabText;
    [SerializeField] GameObject noQuestsText;

    [Header("Forced Texts")]
    [SerializeField] TMP_InputField questInputField;

    [SerializeField] TextMeshProUGUI activeForceTabText;
    [SerializeField] TextMeshProUGUI doneForceTabText;
    [SerializeField] GameObject noForceQuestsText;

    [Space(5)]

    [Header("Rect Transforms")]
    [SerializeField] RectTransform activeQuestsContentRectTransform;
    [SerializeField] RectTransform completedQuestsContentRectTransform;

    [SerializeField] GameObject activeQuestsScrollArea;
    [SerializeField] GameObject completedQuestsScrollArea;

    [Header("Forced Rect Transforms")]
    [SerializeField] RectTransform activeForceQuestsContentRectTransform;
    [SerializeField] RectTransform completedForceQuestsContentRectTransform;

    [SerializeField] GameObject activeForceQuestsScrollArea;
    [SerializeField] GameObject completedForceQuestsScrollArea;

    [Header("Settings")]
    [SerializeField] Color activeTextColor;
    [SerializeField] Color inActiveTextColor;

    // Lists / Dictionaries
    List<Quest> activeQuests = new List<Quest>();
    List<Quest> completedQuests = new List<Quest>();

    List<Quest> activeForceQuests = new List<Quest>();
    List<Quest> completedForceQuests = new List<Quest>();

    Dictionary<Quest, GameObject> questItems = new Dictionary<Quest, GameObject>();
    Dictionary<Quest, GameObject> forceQuestItems = new Dictionary<Quest, GameObject>();

    private void Awake()
    {
        EventSystemNew<Quest>.Subscribe(Event_Type.ACTIVATE_QUEST, ActivateQuest);
        EventSystemNew<Quest>.Subscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);

        EventSystemNew<Quest, int>.Subscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmountToQuest);

        activeTabText.color = activeTextColor;
        doneTabText.color = inActiveTextColor;

        activeForceTabText.color = activeTextColor;
        doneForceTabText.color = inActiveTextColor;
    }

    private void OnDisable()
    {
        EventSystemNew<Quest>.Unsubscribe(Event_Type.ACTIVATE_QUEST, ActivateQuest);
        EventSystemNew<Quest>.Unsubscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);

        EventSystemNew<Quest, int>.Unsubscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmountToQuest);
    }

    public void ToggleTabQuestWindow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (questWindow.activeInHierarchy)
            {
                if (activeTabText.color == activeTextColor)
                {
                    activeTabText.color = inActiveTextColor;
                    doneTabText.color = activeTextColor;

                    if (completedQuests.Count == 0)
                        noQuestsText.SetActive(true);
                    else
                        noQuestsText.SetActive(false);
                }
                else
                {
                    activeTabText.color = activeTextColor;
                    doneTabText.color = inActiveTextColor;

                    if (activeQuests.Count == 0)
                    {
                        noQuestsText.SetActive(true);
                    }
                    else
                    {
                        noQuestsText.SetActive(false);
                    }
                }

                activeQuestsScrollArea.SetActive(!activeQuestsScrollArea.activeInHierarchy);
                completedQuestsScrollArea.SetActive(!completedQuestsScrollArea.activeInHierarchy);
            }
            else if (forceQuestWindow.activeInHierarchy)
            {
                if (activeForceTabText.color == activeTextColor)
                {
                    activeForceTabText.color = inActiveTextColor;
                    doneForceTabText.color = activeTextColor;

                    if (completedForceQuests.Count == 0)
                        noForceQuestsText.SetActive(true);
                    else
                        noForceQuestsText.SetActive(false);
                }
                else
                {
                    activeForceTabText.color = activeTextColor;
                    doneForceTabText.color = inActiveTextColor;

                    if (activeForceQuests.Count == 0)
                        noForceQuestsText.SetActive(true);
                    else
                        noForceQuestsText.SetActive(false);
                }

                activeForceQuestsScrollArea.SetActive(!activeForceQuestsScrollArea.activeInHierarchy);
                completedForceQuestsScrollArea.SetActive(!completedForceQuestsScrollArea.activeInHierarchy);
            }
        }
    }

    public void ToggleQuestWindow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            questWindow.SetActive(!questWindow.activeInHierarchy);
            forceQuestWindow.SetActive(false);

            if (questWindow.activeInHierarchy)
            {
                activeTabText.color = activeTextColor;
                doneTabText.color = inActiveTextColor;

                activeQuestsScrollArea.SetActive(true);
                completedQuestsScrollArea.SetActive(false);

                if (activeQuests.Count == 0)
                    noQuestsText.SetActive(true);
                else
                    noQuestsText.SetActive(false);
            }

            EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, questWindow.activeInHierarchy);
        }
    }

    public void ToggleForceQuestWindow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            questInputField.text = "";
            forceQuestWindow.SetActive(!forceQuestWindow.activeInHierarchy);
            questWindow.SetActive(false);

            if (forceQuestWindow.activeInHierarchy)
            {
                activeForceTabText.color = activeTextColor;
                doneForceTabText.color = inActiveTextColor;

                activeForceQuestsScrollArea.SetActive(true);
                completedForceQuestsScrollArea.SetActive(false);

                if (activeForceQuests.Count == 0)
                    noForceQuestsText.SetActive(true);
                else
                    noForceQuestsText.SetActive(false);
            }

            EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, forceQuestWindow.activeInHierarchy);
        }
    }

    private void ActivateQuest(Quest _quest)
    {
        if (!_quest.isActive && !_quest.isCompleted)
        {
            _quest.isActive = true;
            _quest.isCompleted = false;

            // Quests

            noQuestsInGameText.SetActive(false);

            activeQuests.Add(_quest);

            GameObject newQuestPrefab = Instantiate(questPrefab);

            questItems.Add(_quest, newQuestPrefab);
            
            newQuestPrefab.transform.SetParent(activeQuestsContentRectTransform.transform, false);

            QuestItem questItem = newQuestPrefab.GetComponent<QuestItem>();

            questItem.questName.text = _quest.questName;
            questItem.questDescription.text = _quest.questDescription;

            questItem.UpdateAmount(_quest.questCurrentAmount, _quest.questMaxAmount);

            if (activeQuestsScrollArea.activeInHierarchy)
                noQuestsText.SetActive(false);

            // Force Quests

            activeForceQuests.Add(_quest);

            GameObject newForceQuestPrefab = Instantiate(questPrefab);

            forceQuestItems.Add(_quest, newForceQuestPrefab);

            newForceQuestPrefab.transform.SetParent(activeForceQuestsContentRectTransform.transform, false);

            QuestItem forceQuestItem = newForceQuestPrefab.GetComponent<QuestItem>();

            forceQuestItem.questName.text = _quest.questName;
            forceQuestItem.questDescription.text = _quest.questDescription;

            forceQuestItem.UpdateAmount(_quest.questCurrentAmount, _quest.questMaxAmount);

            if (activeForceQuestsScrollArea.activeInHierarchy)
                noForceQuestsText.SetActive(false);

            foreach (var quest in quests)
            {
                if (quest.quest.questID == _quest.questID)
                {
                    foreach (var questText in quest.questObjects)
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
        if (_quest.isActive && !_quest.isCompleted)
        {
            // Quests

            _quest.isActive = false;
            _quest.isCompleted = true;

            activeQuests.Remove(_quest);
            completedQuests.Add(_quest);

            if (activeQuests.Count == 0)
                noQuestsInGameText.SetActive(true);

            GameObject questObject = questItems[_quest].gameObject;
            QuestItem questItem = questObject.GetComponent<QuestItem>();

            questItem.transform.SetParent(completedQuestsContentRectTransform.transform, false);

            questItem.UpdateAmount(_quest.questMaxAmount, _quest.questMaxAmount);

            if (completedQuestsScrollArea.activeInHierarchy)
                noForceQuestsText.SetActive(false);

            // Force Quests

            activeForceQuests.Remove(_quest);
            completedForceQuests.Add(_quest);

            GameObject forceQuestObject = forceQuestItems[_quest].gameObject;
            QuestItem forceQuestItem = forceQuestObject.GetComponent<QuestItem>();

            forceQuestItem.transform.SetParent(completedForceQuestsContentRectTransform.transform, false);

            forceQuestItem.UpdateAmount(_quest.questMaxAmount, _quest.questMaxAmount);

            if (completedForceQuestsScrollArea.activeInHierarchy)
                noForceQuestsText.SetActive(false);

            foreach (var quest in quests)
            {
                if (quest.quest.questID == _quest.questID)
                {
                    foreach (var questText in quest.questObjects)
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

    public void SetQuestActive()
    {
        if (activeForceQuestsScrollArea.activeInHierarchy)
        {
            foreach (var quest in quests)
            {
                if (quest.quest.questID.ToString() == questInputField.text)
                {
                    EventSystemNew<Quest>.RaiseEvent(Event_Type.ACTIVATE_QUEST, quest.quest);

                    break;
                }
            }
        }
    }

    public void SetQuestCompleted()
    {
        if (completedForceQuestsScrollArea.activeInHierarchy)
        {
            foreach (var quest in quests)
            {
                if (quest.quest.questID.ToString() == questInputField.text)
                {
                    EventSystemNew<Quest>.RaiseEvent(Event_Type.QUEST_COMPLETED, quest.quest);

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
}

[System.Serializable]
public class QuestLibrary
{
    public string questName;

    public Quest quest;

    public GameObject[] questObjects;

    public GameObject[] questCompletionObjects;
}
