using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;

public class BoatController : IInteractable
{
    [Header("References")]
    [SerializeField] PlayerInput playerInput;

    [SerializeField] RobotArmController robotArmController;

    [SerializeField] Transform robotArmLookAt;
    [SerializeField] Transform boatLookAt;

    [SerializeField] CinemachineFreeLook cinemachineCamera;

    [SerializeField] GameObject controlArmText;
    [SerializeField] GameObject exitArmControlText;
    [SerializeField] GameObject exitBoatControlText;

    [SerializeField] GameObject boatHUD;

    [SerializeField] GameObject player;

    [SerializeField] GameObject boatCamera;

    [SerializeField] Transform controlSwitchPlayerPosition;

    [Header("Settings")]
    [SerializeField] float accelerateTime = 2f;
    [SerializeField] float slowDownTime = 2f;
    [SerializeField] float maxVelocity = 5f;
    [SerializeField] float steerSpeed = 5f;
    [SerializeField] float steerSpeedWhenStopped = 5f;

    [SerializeField] float breakTime = 3f;

    Vector3 forwardVelocity = Vector3.zero;

    Vector2 curSteerInput = Vector2.zero;
    Vector2 curMovementInput = Vector2.zero;

    Rigidbody rb;

    bool isControlling = false;

    float currentVelocity;

    bool isMoving = false;

    bool isOnBoat = false;

    bool controllingArm = false;

    Vector3 steerSpeedVector;

    InputActionMap robotArmMap;

    float latestMovementDirection = 0f;

    float smoothDampVelocity;

    private void Awake()
    {
        robotArmMap = playerInput.actions.FindActionMap("RobotArmControls");

        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = transform.position;

        currentVelocity = 0f;

        controlArmText.SetActive(true);
        exitArmControlText.SetActive(false);

        boatHUD.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            currentVelocity = Mathf.SmoothDamp(currentVelocity, maxVelocity * latestMovementDirection, ref smoothDampVelocity, accelerateTime);
        }
        else
        {
            currentVelocity = Mathf.SmoothDamp(currentVelocity, 0f, ref smoothDampVelocity, slowDownTime);
        }

        Vector3 input = (new Vector3(transform.forward.x, 0f, transform.forward.z) * 1f + Vector3.right * 0).normalized * currentVelocity;

        forwardVelocity = input;

        MoveBoat();

        RotateBoat();
    }

    public void EnterDeck(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            if (isControlling && !controllingArm)
            {
                isControlling = !isControlling;

                playerInput.SwitchCurrentActionMap("PlayerControls");

                player.transform.SetParent(null);

                player.transform.position = controlSwitchPlayerPosition.position;

                boatCamera.SetActive(false);

                boatHUD.SetActive(false);

                player.SetActive(true);
            }
        }
    }

    public void ToggleRobotArmControl(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            if (isControlling && robotArmController.CanToggleControl())
            {
                controllingArm = !controllingArm;

                robotArmController.ToggleControl();

                if (controllingArm)
                {
                    controlArmText.SetActive(false);
                    exitArmControlText.SetActive(true);

                    exitBoatControlText.SetActive(false);

                    cinemachineCamera.LookAt = robotArmLookAt;
                    cinemachineCamera.Follow = robotArmLookAt;

                    robotArmMap.Enable();
                }
                else
                {
                    controlArmText.SetActive(true);
                    exitArmControlText.SetActive(false);

                    exitBoatControlText.SetActive(true);

                    cinemachineCamera.LookAt = boatLookAt;
                    cinemachineCamera.Follow = boatLookAt;

                    robotArmMap.Disable();
                }
            }
        }
    }

    public void Moving(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            isMoving = true;

            curMovementInput = _context.ReadValue<Vector2>();
            latestMovementDirection = curMovementInput.y;
        }
        else if (_context.canceled)
        {
            isMoving = false;
        }
    }

    public void Steering(InputAction.CallbackContext _context)
    {
        curSteerInput = _context.ReadValue<Vector2>();
    }

    private void MoveBoat()
    {
        Vector3 up = new Vector3(0f, rb.velocity.y, 0f);

        rb.velocity = forwardVelocity + up;
    }

    private void RotateBoat()
    {
        if (currentVelocity > 1f)
        {
            float newSteerSpeed;

            if (Mathf.Abs(curSteerInput.x) != 0f)
                newSteerSpeed = (steerSpeed * curSteerInput.x * latestMovementDirection) * currentVelocity / maxVelocity;
            else
            {
                newSteerSpeed = 0f;
            }

            steerSpeedVector = new Vector3(0, newSteerSpeed, 0);

            Quaternion deltaRotation = Quaternion.Euler(steerSpeedVector * Time.fixedDeltaTime);

            rb.MoveRotation(rb.rotation * deltaRotation);
        }
        else if (currentVelocity <= 0.1f)
        {
            float newSteerSpeed;

            if (Mathf.Abs(curSteerInput.x) != 0f)
                newSteerSpeed = steerSpeedWhenStopped * curSteerInput.x;
            else
            {
                newSteerSpeed = 0f;
            }

            steerSpeedVector = new Vector3(0, newSteerSpeed, 0);

            Quaternion deltaRotation = Quaternion.Euler(steerSpeedVector * Time.fixedDeltaTime);

            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isControlling)
            {
                isOnBoat = true;

                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isControlling)
            {
                isOnBoat = false;

                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }

    public override string GetInteractPrompt(InteractableTypes _interactableType, string _interactableName)
    {
        if (!isControlling)
        {
            switch (_interactableType)
            {
                case InteractableTypes.Controlable:
                    return string.Format("Control {0}", _interactableName);
                case InteractableTypes.Enterable:
                    if (!isOnBoat)
                        return string.Format("Enter {0}", _interactableName);
                    else
                        return string.Format("Already On {0}", _interactableName);
            }
        }

        return null;
    }

    public override void OnInteract(InteractableTypes _interactableType)
    {
        switch (_interactableType)
        {
            case InteractableTypes.Controlable:
                isControlling = !isControlling;

                if (isControlling)
                {
                    rb.constraints = RigidbodyConstraints.None;

                    playerInput.SwitchCurrentActionMap("BoatControls");

                    player.transform.SetParent(transform);

                    boatCamera.SetActive(true);

                    boatHUD.SetActive(true);

                    exitBoatControlText.SetActive(true);

                    player.SetActive(false);
                }

                break;
            case InteractableTypes.Enterable:
                playerInput.SwitchCurrentActionMap("PlayerControls");

                player.transform.SetParent(null);

                player.transform.position = controlSwitchPlayerPosition.position;

                boatCamera.SetActive(false);
                player.SetActive(true);

                break;
        }
    }
}
