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
    [SerializeField] GameObject questObjectivePrefab;

    [SerializeField] GameObject questWindow;

    [SerializeField] GameObject noQuestsInGameText;

    [Header("Texts")]

    [SerializeField] TextMeshProUGUI activeTabText;
    [SerializeField] TextMeshProUGUI doneTabText;
    [SerializeField] GameObject noActiveQuestsText;
    [SerializeField] GameObject noCompletedQuestsText;

    [SerializeField] GameObject forceQuestRow;
    [SerializeField] TMP_InputField forceQuestInputField;

    [Space(5)]

    [Header("Rect Transforms")]
    [SerializeField] RectTransform activeQuestsContentRectTransform;
    [SerializeField] RectTransform completedQuestsContentRectTransform;

    [SerializeField] RectTransform questsObjectiveContentRectTransform;

    [SerializeField] GameObject activeQuestsScrollArea;
    [SerializeField] GameObject completedQuestsScrollArea;

    [Header("Settings")]
    [SerializeField] Color activeTextColor;
    [SerializeField] Color inActiveTextColor;

    // Lists / Dictionaries
    List<Quest> activeQuests = new List<Quest>();
    List<Quest> completedQuests = new List<Quest>();

    Dictionary<Quest, GameObject> questItems = new Dictionary<Quest, GameObject>();
    Dictionary<Quest, GameObject> questObjectiveItems = new Dictionary<Quest, GameObject>();

    private void Awake()
    {
        activeTabText.color = activeTextColor;
        doneTabText.color = inActiveTextColor;
    }

    private void OnEnable()
    {
        EventSystemNew<Quest>.Subscribe(Event_Type.ACTIVATE_QUEST, ActivateQuest);
        EventSystemNew<int>.Subscribe(Event_Type.ACTIVATE_QUEST, SetQuestActive);

        EventSystemNew<Quest>.Subscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);
        EventSystemNew<int>.Subscribe(Event_Type.QUEST_COMPLETED, SetQuestCompleted);

        EventSystemNew<Quest, int>.Subscribe(Event_Type.QUEST_ADD_AMOUNT, AddAmountToQuest);
    }

    private void OnDisable()
    {
        EventSystemNew<Quest>.Unsubscribe(Event_Type.ACTIVATE_QUEST, ActivateQuest);
        EventSystemNew<int>.Unsubscribe(Event_Type.ACTIVATE_QUEST, SetQuestActive);

        EventSystemNew<Quest>.Unsubscribe(Event_Type.QUEST_COMPLETED, QuestCompleted);
        EventSystemNew<int>.Unsubscribe(Event_Type.QUEST_COMPLETED, SetQuestCompleted);

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
                        noActiveQuestsText.SetActive(true);
                    else
                        noActiveQuestsText.SetActive(false);
                }
                else
                {
                    activeTabText.color = activeTextColor;
                    doneTabText.color = inActiveTextColor;

                    if (activeQuests.Count == 0)
                    {
                        noActiveQuestsText.SetActive(true);
                    }
                    else
                    {
                        noActiveQuestsText.SetActive(false);
                    }
                }

                activeQuestsScrollArea.SetActive(!activeQuestsScrollArea.activeInHierarchy);
                completedQuestsScrollArea.SetActive(!completedQuestsScrollArea.activeInHierarchy);
            }
        }
    }

    public void ToggleQuestWindow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            questWindow.SetActive(!questWindow.activeInHierarchy);

            if (questWindow.activeInHierarchy)
            {
                activeTabText.color = activeTextColor;
                doneTabText.color = inActiveTextColor;

                activeQuestsScrollArea.SetActive(true);
                completedQuestsScrollArea.SetActive(false);

                if (activeQuests.Count == 0)
                    noActiveQuestsText.SetActive(true);
                else
                    noActiveQuestsText.SetActive(false);
            }

            EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, questWindow.activeInHierarchy);
            EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, questWindow.activeInHierarchy);
        }
    }

    public void ToggleForceQuestInputField(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (questWindow.activeInHierarchy)
            {
                forceQuestRow.gameObject.SetActive(!forceQuestRow.gameObject.activeInHierarchy);
                forceQuestInputField.text = "";
            }
        }
    }

    private void ActivateQuest(Quest _quest)
    {
        if (!activeQuests.Contains(_quest) && !completedQuests.Contains(_quest))
        {
            _quest.questCurrentAmount = 0;

            noQuestsInGameText.SetActive(false);

            activeQuests.Add(_quest);

            // Quest Window Item
            GameObject newQuestPrefab = Instantiate(questPrefab);

            questItems.Add(_quest, newQuestPrefab);
            
            newQuestPrefab.transform.SetParent(activeQuestsContentRectTransform.transform, false);

            QuestItem questItem = newQuestPrefab.GetComponent<QuestItem>();

            questItem.questName.text = _quest.questName;
            questItem.questDescription.text = _quest.questDescription;

            questItem.UpdateAmount(_quest.questCurrentAmount, _quest.questMaxAmount);

            // Quest UI Objective
            GameObject newQuestObjectivePrefab = Instantiate(questObjectivePrefab);

            questObjectiveItems.Add(_quest, newQuestObjectivePrefab);

            newQuestObjectivePrefab.transform.SetParent(questsObjectiveContentRectTransform.transform, false);

            QuestObjectiveItem questObjectiveItem = newQuestObjectivePrefab.GetComponent<QuestObjectiveItem>();

            questObjectiveItem.SetObjective(_quest.questObjective);

            questObjectiveItem.UpdateAmount(_quest.questCurrentAmount, _quest.questMaxAmount);

            if (activeQuestsScrollArea.activeInHierarchy)
                noActiveQuestsText.SetActive(false);

            foreach (var quest in quests)
            {
                if (quest.quest == _quest)
                {
                    foreach (var qObject in quest.questObjects)
                    {
                        qObject.SetActive(true);
                    }

                    break;
                }
            }
        }
    }

    private void QuestCompleted(Quest _quest)
    {
        if (activeQuests.Contains(_quest))
        {
            activeQuests.Remove(_quest);
            completedQuests.Add(_quest);

            if (activeQuests.Count == 0)
                noQuestsInGameText.SetActive(true);

            Destroy(questObjectiveItems[_quest].gameObject);

            GameObject questObject = questItems[_quest].gameObject;
            QuestItem questItem = questObject.GetComponent<QuestItem>();

            questItem.transform.SetParent(completedQuestsContentRectTransform.transform, false);

            questItem.UpdateAmount(_quest.questMaxAmount, _quest.questMaxAmount);

            if (completedQuestsScrollArea.activeInHierarchy)
                noActiveQuestsText.SetActive(false);

            foreach (var quest in quests)
            {
                if (quest.quest == _quest)
                {
                    foreach (var qObject in quest.questObjects)
                    {
                        qObject.SetActive(false);
                    }

                    foreach (var qCompletionObject in quest.questCompletionObjects)
                    {
                        qCompletionObject.SetActive(true);
                    }

                    break;
                }
            }
        }
    }

    public void SetQuestActive(int _questID)
    {
        foreach (var quest in quests)
        {
            if (quest.quest.questID == _questID)
            {
                EventSystemNew<Quest>.RaiseEvent(Event_Type.ACTIVATE_QUEST, quest.quest);

                break;
            }
        }
    }

    public void SetQuestActive()
    {
        if (activeQuestsScrollArea.activeInHierarchy)
        {
            foreach (var quest in quests)
            {
                if (quest.quest.questID.ToString() == forceQuestInputField.text)
                {
                    EventSystemNew<Quest>.RaiseEvent(Event_Type.ACTIVATE_QUEST, quest.quest);

                    break;
                }
            }
        }
    }

    public void SetQuestCompleted(int _questID)
    {
        foreach (var quest in quests)
        {
            if (quest.quest.questID == _questID)
            {
                if (activeQuests.Contains(quest.quest))
                {
                    EventSystemNew<Quest>.RaiseEvent(Event_Type.QUEST_COMPLETED, quest.quest);
                }

                break;
            }
        }
    }

    public void SetQuestCompleted()
    {
        if (completedQuestsScrollArea.activeInHierarchy)
        {
            foreach (var quest in quests)
            {
                if (quest.quest.questID.ToString() == forceQuestInputField.text)
                {
                    if (activeQuests.Contains(quest.quest))
                    {
                        EventSystemNew<Quest>.RaiseEvent(Event_Type.QUEST_COMPLETED, quest.quest);
                    }

                    break;
                }
            }
        }
    }

    private void AddAmountToQuest(Quest _quest, int _amount)
    {
        if (activeQuests.Contains(_quest))
        {
            _quest.questCurrentAmount += _amount;

            if (_quest.questCurrentAmount >= _quest.questMaxAmount)
            {
                EventSystemNew<Quest>.RaiseEvent(Event_Type.QUEST_COMPLETED, _quest);
            }
            else
            {
                GameObject questObject = questItems[_quest].gameObject;
                QuestItem questItem = questObject.GetComponent<QuestItem>();

                questItem.UpdateAmount(_quest.questCurrentAmount, _quest.questMaxAmount);

                GameObject questObjective = questObjectiveItems[_quest].gameObject;
                QuestObjectiveItem questObjectiveitem = questObjective.GetComponent<QuestObjectiveItem>();

                questObjectiveitem.UpdateAmount(_quest.questCurrentAmount, _quest.questMaxAmount);
            }
        }
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
