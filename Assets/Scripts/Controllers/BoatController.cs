using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;
using TMPro;

public class BoatController : MonoBehaviour //: IInteractable
{
    [Header("References")]
    [SerializeField] PlayerInput playerInput;

    [SerializeField] RobotArmController robotArmController;

    [SerializeField] Transform playerControlPoint;

    [SerializeField] Transform robotArmLookAt;
    [SerializeField] Transform boatLookAt;

    [SerializeField] CinemachineFreeLook cinemachineCamera;

    [SerializeField] GameObject enterDeckText;
    [SerializeField] GameObject controlText;

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
    [SerializeField] float steerDamping = 5f;

    Vector3 forwardVelocity = Vector3.zero;

    Vector2 curSteerInput = Vector2.zero;
    Vector2 curMovementInput = Vector2.zero;

    [SerializeField] Rigidbody rb;

    bool isControlling = false;

    float currentVelocity;

    bool isMoving = false;

    bool isOnBoat = false;

    bool isNearBoat = false;

    bool controllingArm = false;

    Vector3 steerSpeedVector;

    InputActionMap robotArmMap;

    float latestMovementDirection = 0f;

    float smoothDampVelocity;

    FreezeActions freezeActionsManager;

    float newYRotation = 0f;

    CharacterController playerController;

    private void OnEnable()
    {
        EventSystemNew<bool>.Subscribe(Event_Type.PLAYER_NEAR_BOAT, IsPlayerNearBoat);
    }

    private void OnDisable()
    {
        EventSystemNew<bool>.Unsubscribe(Event_Type.PLAYER_NEAR_BOAT, IsPlayerNearBoat);
    }

    private void Awake()
    {
        robotArmMap = playerInput.actions.FindActionMap("RobotArmControls");

        rb.centerOfMass = transform.position;

        currentVelocity = 0f;

        controlArmText.SetActive(true);
        exitArmControlText.SetActive(false);

        boatHUD.SetActive(false);
    }

    private void Start()
    {
        freezeActionsManager = FreezeActions.Instance;

        playerController = player.GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isNearBoat && !isControlling && !isOnBoat)
        {
            enterDeckText.SetActive(true);
        }
        else
        {
            enterDeckText.SetActive(false);
        }

        if (!isControlling && isOnBoat)
        {
            controlText.SetActive(true);
        }
        else
        {
            controlText.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (isControlling)
        {
            player.transform.position = playerControlPoint.position;
            player.transform.rotation = playerControlPoint.rotation;
        }

        if (isMoving && !freezeActionsManager.isFrozen)
        {
            currentVelocity = Mathf.SmoothDamp(currentVelocity, maxVelocity * latestMovementDirection, ref smoothDampVelocity, accelerateTime);
        }
        else if (!isMoving || freezeActionsManager.isFrozen)
        {
            currentVelocity = Mathf.SmoothDamp(currentVelocity, 0f, ref smoothDampVelocity, slowDownTime);
        }

        Vector3 input = (new Vector3(transform.forward.x, 0f, transform.forward.z) * 1f + Vector3.right * 0).normalized * currentVelocity;

        forwardVelocity = input;

        MoveBoat();

        RotateBoat();
    }

    private void IsPlayerNearBoat(bool _isNearBoat)
    {
        isNearBoat = _isNearBoat;
    }

    public void EnterDeck(InputAction.CallbackContext _context)
    {
        if (_context.performed && !freezeActionsManager.isFrozen && !freezeActionsManager.isPositionFrozen)
        {
            if (!isOnBoat && isNearBoat)
            {
                enterDeckText.SetActive(false);

                playerInput.SwitchCurrentActionMap("PlayerControls");

                player.transform.SetParent(null);

                playerController.enabled = false;
                player.transform.position = controlSwitchPlayerPosition.position;
                playerController.enabled = true;

                boatCamera.SetActive(false);

                boatHUD.SetActive(false);

                //player.SetActive(true);
            }
        }
    }

    public void ToggleBoatControl(InputAction.CallbackContext _context)
    {
        if (_context.performed && !freezeActionsManager.isFrozen && !freezeActionsManager.isPositionFrozen)
        {
            if (isOnBoat)
            {
                isControlling = !isControlling;

                if (isControlling)
                {
                    playerControlPoint.position = player.transform.position;
                    playerControlPoint.rotation = player.transform.rotation;

                    Debug.Log("Transform: " + player.transform.position);

                    player.GetComponent<ThirdPersonController>().canMove = false;

                    rb.constraints = RigidbodyConstraints.None;

                    playerInput.SwitchCurrentActionMap("BoatControls");

                    player.transform.SetParent(transform, true);

                    boatCamera.SetActive(true);

                    boatHUD.SetActive(true);

                    exitBoatControlText.SetActive(true);
                }
                else
                {
                    player.transform.position = playerControlPoint.position;
                    player.transform.rotation = playerControlPoint.rotation;

                    player.GetComponent<ThirdPersonController>().canMove = true;

                    playerInput.SwitchCurrentActionMap("PlayerControls");

                    player.transform.SetParent(null);

                    boatCamera.SetActive(false);

                    boatHUD.SetActive(false);
                }
            }

            //player.SetActive(false);
        }
    }

    //public void EnterDeck(InputAction.CallbackContext _context)
    //{
    //    if (_context.performed && !freezeActionsManager.isFrozen)
    //    {
    //        if (isControlling && !controllingArm)
    //        {
    //            isControlling = !isControlling;

    //            playerInput.SwitchCurrentActionMap("PlayerControls");

    //            player.transform.SetParent(null);

    //            player.transform.position = controlSwitchPlayerPosition.position;

    //            boatCamera.SetActive(false);

    //            boatHUD.SetActive(false);

    //            player.SetActive(true);
    //        }
    //    }
    //}

    public void ToggleRobotArmControl(InputAction.CallbackContext _context)
    {
        if (_context.performed && !freezeActionsManager.isFrozen)
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
        if (_context.performed && !freezeActionsManager.isFrozen)
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
        if (freezeActionsManager.isFrozen)
            return;

        float newSteerSpeed = 0f;

        newSteerSpeed = (steerSpeed * curSteerInput.x) * currentVelocity / maxVelocity;

        newYRotation = Mathf.Lerp(newYRotation, newSteerSpeed, Time.fixedDeltaTime * steerDamping);

        steerSpeedVector = new Vector3(0, newYRotation, 0);

        Quaternion deltaRotation = Quaternion.Euler(steerSpeedVector * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation * deltaRotation);
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

            if (isControlling)
            {
                playerController.Move(rb.velocity * Time.deltaTime);
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

    //public override string GetInteractPrompt(InteractableTypes _interactableType, string _interactableName)
    //{
    //    if (!isControlling && !freezeActionsManager.isFrozen)
    //    {
    //        switch (_interactableType)
    //        {
    //            case InteractableTypes.Controlable:
    //                return string.Format("Control {0}", _interactableName);
    //            case InteractableTypes.Enterable:
    //                if (!isOnBoat)
    //                    return string.Format("Enter {0}", _interactableName);
    //                else
    //                    return string.Format("Already On {0}", _interactableName);
    //        }
    //    }

    //    return null;
    //}

    //public override void OnInteract(InteractableTypes _interactableType)
    //{
    //    if (freezeActionsManager.isFrozen)
    //        return;

    //    switch (_interactableType)
    //    {
    //        case InteractableTypes.Controlable:
    //            isControlling = !isControlling;

    //            if (isControlling)
    //            {
    //                rb.constraints = RigidbodyConstraints.None;

    //                playerInput.SwitchCurrentActionMap("BoatControls");

    //                player.transform.SetParent(transform);

    //                boatCamera.SetActive(true);

    //                boatHUD.SetActive(true);

    //                exitBoatControlText.SetActive(true);

    //                player.SetActive(false);
    //            }

    //            break;
    //        case InteractableTypes.Enterable:
    //            playerInput.SwitchCurrentActionMap("PlayerControls");

    //            player.transform.SetParent(null);

    //            player.transform.position = controlSwitchPlayerPosition.position;

    //            boatCamera.SetActive(false);
    //            player.SetActive(true);

    //            break;
    //    }
    //}
}
