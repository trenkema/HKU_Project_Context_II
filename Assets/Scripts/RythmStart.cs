using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RythmStart : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Quest quest;

    [SerializeField] GameObject rythmCamera;

    [SerializeField] RythmMinigame rythmMinigame;

    [SerializeField] GameObject startPumpingText;

    [SerializeField] GameObject pumpArrow;

    bool isQuestActive = false;

    bool isInTrigger = false;
    bool isPlaying = false;

    bool pressedButtonStart = false;
    bool pressedButtonExit = false;

    private void OnEnable()
    {
        EventSystemNew<Quest>.Subscribe(Event_Type.ACTIVATE_QUEST, QuestActive);
    }

    private void OnDisable()
    {
        EventSystemNew<Quest>.Unsubscribe(Event_Type.ACTIVATE_QUEST, QuestActive);
    }

    private void Start()
    {
        rythmCamera.SetActive(false);
        startPumpingText.SetActive(false);
    }

    private void QuestActive(Quest _quest)
    {
        if (quest == _quest)
        {
            isQuestActive = true;

            pumpArrow.SetActive(true);
        }
    }

    public void StartMiniGame(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started && isInTrigger & !pressedButtonExit && !isPlaying)
        {
            pumpArrow.SetActive(false);

            pressedButtonStart = true;

            rythmCamera.SetActive(true);

            isPlaying = true;

            rythmMinigame.StartGame();

            EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, true);
        }
        if (_context.phase == InputActionPhase.Canceled && pressedButtonStart)
        {
            pressedButtonStart = false;
        }
    }

    public void ExitMiniGame(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started && !pressedButtonStart && isPlaying)
        {
            pumpArrow.SetActive(true);

            pressedButtonExit = true;

            rythmCamera.SetActive(false);

            isPlaying = false;

            rythmMinigame.ExitGame();

            EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, false);
        }
        if (_context.phase == InputActionPhase.Canceled && pressedButtonExit)
        {
            pressedButtonExit = false;
        }
    }

    public void MiniGameFinished()
    {
        pumpArrow.SetActive(false);

        rythmCamera.SetActive(false);

        isPlaying = false;

        rythmMinigame.ExitGame();

        isQuestActive = false;

        isInTrigger = false;

        EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isQuestActive)
        {
            isInTrigger = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isPlaying && isQuestActive)
        {
            startPumpingText.SetActive(true);
        }
        else
        {
            startPumpingText.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;

            startPumpingText.SetActive(false);
        }
    }
}
