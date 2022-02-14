using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerCrosshair;

    [SerializeField] GameObject mainMenuHUD;

    [SerializeField] Animator mainMenuAnimator;

    [SerializeField] Animator questAnimator;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] float fadeDuration;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerCrosshair.SetActive(false);
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mainMenuAnimator.SetBool("FadeOut", true);

        questAnimator.SetBool("FadeIn", true);

        Invoke("Started", fadeDuration);
    }

    private void Started()
    {
        playerCrosshair.SetActive(true);

        playerInput.SwitchCurrentActionMap("PlayerControls");

        mainMenuHUD.SetActive(false);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
