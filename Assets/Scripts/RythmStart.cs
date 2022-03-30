using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RythmStart : MonoBehaviour
{
    [SerializeField] RythmMinigame rythmMinigame;

    [SerializeField] GameObject startPumpingText;

    bool isInTrigger = false;
    bool isPlaying = false;

    bool pressedButtonStart = false;
    bool pressedButtonExit = false;

    public void StartMiniGame(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started && isInTrigger & !pressedButtonExit && !isPlaying)
        {
            pressedButtonStart = true;

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
                pressedButtonExit = true;

                isPlaying = false;

                rythmMinigame.ExitGame();

                EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, false);
            }
            if (_context.phase == InputActionPhase.Canceled && pressedButtonExit)
            {
                pressedButtonExit = false;
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isPlaying)
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
