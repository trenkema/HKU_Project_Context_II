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
    float accelerateLerpTimeElapsed;
    float accelerateLerpedValue;

    float slowDownLerpTimeElapsed;
    float slowDownLerpedValue;

    float breakLerpTimeElapsed;
    float breakLerpedValue;

    bool isMoving = false;

    bool isOnBoat = false;

    bool controllingArm = false;

    Vector3 steerSpeedVector;

    InputActionMap robotArmMap;

    float latestMovementDirection = 0f;

    private void Awake()
    {
        robotArmMap = playerInput.actions.FindActionMap("RobotArmControls");

        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = transform.position;

        slowDownLerpTimeElapsed = slowDownTime;

        currentVelocity = 0f;

        controlArmText.SetActive(true);
        exitArmControlText.SetActive(false);

        boatHUD.SetActive(false);
    }

    private void Update()
    {
        if (isMoving)
        {
            if ((latestMovementDirection > 0f && rb.velocity.z < -0.5f) || (latestMovementDirection < 0f && rb.velocity.z > 0.5f))
            {
                if (breakLerpTimeElapsed < breakTime)
                {
                    breakLerpedValue = Mathf.Lerp(currentVelocity, 0f, breakLerpTimeElapsed / breakTime);

                    breakLerpTimeElapsed += Time.deltaTime;
                }
                else
                {
                    breakLerpTimeElapsed = breakTime;

                    breakLerpedValue = 0f;
                }

                float direction;

                if (rb.velocity.z > 0f)
                    direction = 1f;
                else
                    direction = -1f;

                Vector3 input = (transform.forward * 1f + transform.right * 0).normalized * (breakLerpedValue * direction);

                currentVelocity = breakLerpedValue;

                forwardVelocity = input;
            }
            else
            {
                breakLerpTimeElapsed = 0f;

                slowDownLerpTimeElapsed = 0f;

                if (accelerateLerpTimeElapsed < accelerateTime)
                {
                    accelerateLerpedValue = Mathf.Lerp(currentVelocity, maxVelocity, accelerateLerpTimeElapsed / accelerateTime);

                    accelerateLerpTimeElapsed += Time.deltaTime;
                }
                else
                {
                    accelerateLerpTimeElapsed = accelerateTime;

                    accelerateLerpedValue = maxVelocity;
                }

                Vector3 input = (transform.forward * 1f + transform.right * 0).normalized * (accelerateLerpedValue * latestMovementDirection);

                currentVelocity = accelerateLerpedValue;

                forwardVelocity = input;
            }
        }
        else
        {
            accelerateLerpTimeElapsed = 0f;

            if (slowDownLerpTimeElapsed < slowDownTime)
            {
                slowDownLerpedValue = Mathf.Lerp(currentVelocity, 0f, slowDownLerpTimeElapsed / slowDownTime);

                slowDownLerpTimeElapsed += Time.deltaTime;
            }
            else
            {
                slowDownLerpTimeElapsed = slowDownTime;

                slowDownLerpedValue = 0f;
            }

            Vector3 input = (transform.forward * 1f + transform.right * 0).normalized * (slowDownLerpedValue * latestMovementDirection);

            currentVelocity = slowDownLerpedValue;

            forwardVelocity = input;
        }
    }

    private void FixedUpdate()
    {
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

                    cinemachineCamera.LookAt = robotArmLookAt;
                    cinemachineCamera.Follow = robotArmLookAt;

                    robotArmMap.Enable();
                }
                else
                {
                    controlArmText.SetActive(true);
                    exitArmControlText.SetActive(false);

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
