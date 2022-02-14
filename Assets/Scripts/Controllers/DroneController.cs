using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class DroneController : IInteractable
{
    [Header("References")]
    [SerializeField] Quest quest;

    [SerializeField] Animator animator;

    [SerializeField] Transform seedsDropTransform;
    [SerializeField] GameObject seedPrefab;

    [SerializeField] NoPlayerInteraction noPlayerInteraction;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] GameObject droneCamera;
    [SerializeField] GameObject droneHUD;

    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject playerHUD;

    [Header("Settings")]
    [SerializeField] float flyUpForce = 10f;
    [SerializeField] float flyDownForce = 10f;
    [SerializeField] float flyStillForce = 10f;

    [SerializeField] float movementForwardSpeed = 200f;

    [SerializeField] float tiltAmountForward = 20f;
    [SerializeField] float rotateAmountByKeys = 2.5f;

    Vector2 curMovementInput;
    Vector2 curRotationInput;

    bool isControlling = false;

    float upForce = 98.1f;

    float forwardForce = 0f;

    float tiltVelocityForward;

    float wantedYRotation;
    float currentYRotation;
    float rotationYVelocity;

    bool canDropSeeds = false;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = transform.position;

        upForce = 0f;

        EventSystemNew<int>.Subscribe(Event_Type.ENABLE_QUEST, EnableQuest);
    }

    private void OnDisable()
    {
        EventSystemNew<int>.Unsubscribe(Event_Type.ENABLE_QUEST, EnableQuest);
    }

    private void Update()
    {
        if (!isControlling)
        {
            animator.SetBool("isFlying", false);

            upForce = 0f;

            forwardForce = 0f;
        }
    }

    private void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.up * upForce);

        rb.AddRelativeForce(Vector3.forward * curMovementInput.y * forwardForce);

        if (curMovementInput.y != 0f)
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20f * curMovementInput.y, ref tiltVelocityForward, 0.1f);
        else
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 0f, ref tiltVelocityForward, 0.1f);

        wantedYRotation += rotateAmountByKeys * curRotationInput.x;

        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
        
        rb.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, rb.rotation.z));
    }

    private void EnableQuest(int _questID)
    {
        if (quest.questID == _questID)
        {
            canDropSeeds = true;
        }
        else
        {
            canDropSeeds = false;
        }
    }

    public void OnDropSeeds(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (canDropSeeds)
            {
                GameObject seed = Instantiate(seedPrefab, seedsDropTransform.position, Quaternion.identity);
            }
        }
    }

    public void OnFlyUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (isControlling)
            {
                animator.SetBool("isFlying", true);

                upForce = flyUpForce;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (isControlling)
                upForce = flyStillForce;
        }
    }

    public void OnFlyDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (isControlling)
            {
                animator.SetBool("isFlying", true);

                upForce = flyDownForce;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (isControlling)
                upForce = flyStillForce;
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (isControlling)
            {
                curMovementInput = context.ReadValue<Vector2>();

                forwardForce = movementForwardSpeed;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (isControlling)
            {
                curMovementInput = context.ReadValue<Vector2>();

                forwardForce = 0f;
            }
        }
    }

    public void OnRotation(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (isControlling)
            {
                curRotationInput = context.ReadValue<Vector2>();
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (isControlling)
            {
                curRotationInput = context.ReadValue<Vector2>();
            }
        }
    }

    public void ExitDrone(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            if (isControlling)
            {
                noPlayerInteraction.DisableScript(false);

                isControlling = !isControlling;

                playerInput.SwitchCurrentActionMap("PlayerControls");

                playerCamera.SetActive(true);

                playerHUD.SetActive(true);

                droneCamera.SetActive(false);

                droneHUD.SetActive(false);
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
            }
        }

        return null;
    }

    public override void OnInteract(InteractableTypes _interactableType)
    {
        if (_interactableType == InteractableTypes.Controlable)
        {
            isControlling = !isControlling;

            if (isControlling)
            {
                noPlayerInteraction.DisableScript(true);

                rb.constraints = RigidbodyConstraints.None;

                playerInput.SwitchCurrentActionMap("DroneControls");

                playerCamera.SetActive(false);

                playerHUD.SetActive(false);

                droneCamera.SetActive(true);

                droneHUD.SetActive(true);
            }
        }
    }
}
