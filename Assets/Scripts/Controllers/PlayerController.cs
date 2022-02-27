using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;

    [SerializeField] float jumpForce;

    [SerializeField] Rigidbody rb;

    [Header("Player Look")]
    [SerializeField] Transform playerCamera;
    [SerializeField] CinemachineVirtualCamera playerCinemachine;
    [SerializeField] CinemachinePOV playerCinemachinePOV;
    [SerializeField] float minXLook;
    [SerializeField] float maxXLook;
    [SerializeField] float lookSensitivity;

    [SerializeField] float smoothInputSpeed = 0.2f;

    [Header("Player GroundCheck")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] LayerMask groundLayer;

    public Vector2 curMovementInput { private set; get; }
    Vector2 smoothInputVelocity;

    bool isMoving = false;

    public bool isSprinting { private set; get; }

    bool isGrounded = true;

    FreezeActions freezeActionsManager;

    float playerCameraSpeedX;
    float playerCameraSpeedY;

    private void Start()
    {
        isSprinting = false;

        freezeActionsManager = FreezeActions.Instance;

        playerCinemachinePOV = playerCinemachine.GetCinemachineComponent<CinemachinePOV>();

        playerCameraSpeedX = playerCinemachinePOV.m_HorizontalAxis.m_MaxSpeed;
        playerCameraSpeedY = playerCinemachinePOV.m_VerticalAxis.m_MaxSpeed;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void FixedUpdate()
    {
        Move();

        if (freezeActionsManager.isFrozen)
        {
            playerCinemachinePOV.m_HorizontalAxis.m_MaxSpeed = 0f;
            playerCinemachinePOV.m_VerticalAxis.m_MaxSpeed = 0f;
        }
        else
        {
            playerCinemachinePOV.m_HorizontalAxis.m_MaxSpeed = playerCameraSpeedX;
            playerCinemachinePOV.m_VerticalAxis.m_MaxSpeed = playerCameraSpeedY;
        }
    }

    private void Move()
    {
        if (!isMoving || freezeActionsManager.isFrozen)
        {
            curMovementInput = Vector2.SmoothDamp(curMovementInput, Vector2.zero, ref smoothInputVelocity, smoothInputSpeed);
        }

        float newSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 direction = playerCamera.forward * curMovementInput.y + playerCamera.right * curMovementInput.x;
        direction *= newSpeed;
        direction.y = rb.velocity.y;

        rb.velocity = direction;
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
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isSprinting = false;
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
