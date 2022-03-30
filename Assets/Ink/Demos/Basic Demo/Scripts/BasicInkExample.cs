using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Ink.Runtime;
using TMPro;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;

    public bool npcTalking = false;

    [SerializeField]
    private TextAsset inkJSONAsset = null;
    public Story story;
    [SerializeField]
    private RectTransform textParent = null;
    [SerializeField]
    private RectTransform choiceParent = null;
    // UI Prefabs
    [SerializeField]
    private TextMeshProUGUI textPrefab = null;
    [SerializeField]
    private GameObject buttonPrefab = null;
    [SerializeField]
    private TextMeshProUGUI npcName = null;
    [SerializeField]
    private npcScriptableObject[] npcs;
    [SerializeField]
    private GameObject hud;

    string onTriggerNPCName = "";
    Animator onTriggerNPCAnimator;

    void Awake()
    {
        // Remove the default message
        RemoveChildren();
        StartStory();

        ToggleHUD(false);
    }

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.TALK_TO_NPC, StartStoryFromNPC);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.TALK_TO_NPC, StartStoryFromNPC);
    }

    public void OnTriggerNPC(string npcName, Animator npcAnimator)
    {
        onTriggerNPCName = npcName;
        onTriggerNPCAnimator = npcAnimator;
    }

    public void StartStoryFromNPC()
    {
        if (onTriggerNPCName != string.Empty && !npcTalking)
        {
            npcTalking = true;

            EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, true);
            EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, true);

            story.ChoosePathString(onTriggerNPCName);

            ToggleHUD(true);

            RefreshView();
        }
    }

    void ToggleHUD(bool isActive)
    {
        hud.SetActive(isActive);
    }

    // Creates a new Story object with the compiled story which we can then play!
    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null) OnCreateStory(story);
        story.BindExternalFunction("isTalking", (string name) =>
        {
            EventSystemNew<string>.RaiseEvent(Event_Type.TALK_TO_NPC, name);

            isTalking(name);
        });
        story.BindExternalFunction("completeQuest", (int questId) =>
                {
                    EventSystemNew<int>.RaiseEvent(Event_Type.QUEST_COMPLETED, questId);
                });
        story.BindExternalFunction("startQuest", (int questId) =>
                {
                    EventSystemNew<int>.RaiseEvent(Event_Type.ACTIVATE_QUEST, questId);
                });

        RefreshView();
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    void RefreshView()
    {
        // Remove all the UI on screen
        RemoveChildren();

        // Read all the content until we can't continue any more
        while (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            CreateContentView(text);
        }

        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate
                {
                    OnClickChoiceButton(choice);
                });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            Button choice = CreateChoiceView("Continue");
            choice.onClick.AddListener(delegate
            {
                onTriggerNPCAnimator.SetBool("isSpeaking", false);

                ToggleHUD(false);

                EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, false);
                EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, false);

                npcTalking = false;
            });

            //Button choice = CreateChoiceView("End of story.\nRestart?");
            //choice.onClick.AddListener(delegate
            //{
            //    StartStory();
            //});
        }
    }

    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton(Choice choice)
    {
        onTriggerNPCAnimator.SetBool("isSpeaking", false);
        Invoke("StartSpeaking", 0.1f);

        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }

    void StartSpeaking()
    {
        onTriggerNPCAnimator.SetBool("isSpeaking", true);
    }

    // Creates a textbox showing the the line of text
    void CreateContentView(string text)
    {
        TextMeshProUGUI storyText = Instantiate(textPrefab) as TextMeshProUGUI;
        storyText.text = text;
        storyText.transform.SetParent(textParent.transform, false);
    }

    //IEnumerator ScrollText(TextMeshProUGUI storyText, string text)
    //{
    //    for(int i = 0; i < text.Length; i++)
    //    {
    //        string currentText = text.Substring(0, i);
    //        storyText.text = currentText;
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //}

    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        GameObject buttonParent = Instantiate(buttonPrefab, choiceParent.transform);
        Button choice = buttonParent.GetComponentInChildren<Button>();
        //choice.transform.SetParent(choiceParent.transform, false);

        // Gets the text from the button prefab
        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
        choiceText.text = text;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    // Destroys all the children of this gameobject (all the UI)
    void RemoveChildren()
    {
        int childCount = textParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(textParent.transform.GetChild(i).gameObject);
        }
        childCount = choiceParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(choiceParent.transform.GetChild(i).gameObject);
        }
    }

    void isTalking(string name)
    {
        bool hasFoundName = false;

        foreach (npcScriptableObject npc in npcs)
        {
            if (npc.npcName == name)
            {
                npcName.text = npc.npcName;
                hasFoundName = true;
                Debug.Log(name + " is talking");
            }
        }
        if (hasFoundName == false)
        {
            Debug.Log("<color=red>Error: There is no NPC asset with the name " + name + " found.</color>");
            Debug.Break();
        }
    }
}
