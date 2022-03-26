using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuCamera;

    [SerializeField] GameObject postProcessingMainMenu;
    [SerializeField] GameObject postProcessingInGame;

    [SerializeField] Quest temporaryStartQuest;

    [SerializeField] GameObject mainMenuHUD;

    [SerializeField] Animator mainMenuAnimator;

    [SerializeField] Animator fadeAnimator;

    [SerializeField] Animator inGameUIAnimator;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] float fadeDuration;

    FreezeActions freezeActionsManager;

    InputActionMap uiMap;

    private void Start()
    {
        freezeActionsManager = FreezeActions.Instance;

        uiMap = playerInput.actions.FindActionMap("UI");

        EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, true); // Freeze Movement At Start
        EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, true);
    }

    private void OnEnable()
    {
        if (temporaryStartQuest != null)
            EventSystemNew<Quest>.RaiseEvent(Event_Type.ACTIVATE_QUEST, temporaryStartQuest);

        EventSystemNew<bool>.Subscribe(Event_Type.CURSOR_ON, CursorOn);
    }

    private void OnDisable()
    {
        EventSystemNew<bool>.Unsubscribe(Event_Type.CURSOR_ON, CursorOn);
    }

    public void OnFreezeToggle(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            bool isFrozen = freezeActionsManager.isFrozen;

            EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, !isFrozen);
            EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, !isFrozen);
        }
    }

    private void CursorOn(bool _toggleOn)
    {
        if (_toggleOn)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mainMenuCamera.SetActive(false);

        mainMenuAnimator.SetBool("FadeOut", true);

        inGameUIAnimator.SetBool("FadeIn", true);

        Invoke("Started", fadeDuration);
    }

    private void Started()
    {
        EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, false);
        EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, false);

        playerInput.SwitchCurrentActionMap("PlayerControls");

        uiMap.Enable();

        mainMenuHUD.SetActive(false);

        postProcessingMainMenu.SetActive(false);
        postProcessingInGame.SetActive(true);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
