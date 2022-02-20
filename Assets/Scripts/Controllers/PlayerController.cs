using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;

    [SerializeField] float jumpForce;

    [SerializeField] Rigidbody rb;

    [Header("Player Look")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform playerTransform;
    [SerializeField] float minXLook;
    [SerializeField] float maxXLook;
    [SerializeField] float lookSensitivity;

    [SerializeField] float smoothInputSpeed = 0.2f;

    [Header("Player GroundCheck")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] LayerMask groundLayer;

    float curCamRotX;
    Vector2 mouseDelta;

    public Vector2 curMovementInput { private set; get; }
    Vector2 smoothInputVelocity;

    bool isMoving = false;

    public bool isSprinting { private set; get; }

    bool isGrounded = true;

    FreezeActions freezeActionsManager;

    private void Start()
    {
        isSprinting = false;

        freezeActionsManager = FreezeActions.Instance;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!isMoving || freezeActionsManager.isFrozen)
        {
            curMovementInput = Vector2.SmoothDamp(curMovementInput, Vector2.zero, ref smoothInputVelocity, smoothInputSpeed);
        }

        float newSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 direction = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        direction *= newSpeed;
        direction.y = rb.velocity.y;

        rb.velocity = direction;
    }

    private void CameraLook()
    {
        if (freezeActionsManager.isFrozen)
            return;

        curCamRotX += mouseDelta.y * lookSensitivity;
        curCamRotX = Mathf.Clamp(curCamRotX, minXLook, maxXLook);
        cameraHolder.localEulerAngles = new Vector3(-curCamRotX, 0, 0);

        playerTransform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && !freezeActionsManager.isFrozen)
        {
            isMoving = true;
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isMoving = false;
        }
    }

    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !freezeActionsManager.isFrozen)
        {
            isSprinting = true;

            Debug.Log("Sprinting");
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isSprinting = false;

            Debug.Log("Stopped Sprinting");
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !freezeActionsManager.isFrozen)
        {
            if (isGrounded)
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
