using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float jumpForce;

    [Header("Player Look")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    [SerializeField] private float lookSensitivity;

    [SerializeField] private float smoothInputSpeed = 0.2f;

    private float curCamRotX;
    private Vector2 mouseDelta;

    [SerializeField] Rigidbody rb;

    public bool canLook = true;
    private Vector2 curMovementInput;
    Vector2 smoothInputVelocity;

    private bool isMoving = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        if (!isMoving)
        {
            curMovementInput = Vector2.SmoothDamp(curMovementInput, Vector2.zero, ref smoothInputVelocity, smoothInputSpeed);
        }

        Vector3 direction = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        direction *= moveSpeed;
        direction.y = rb.velocity.y;

        rb.velocity = direction;
    }

    private void CameraLook()
    {
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
        if (context.phase == InputActionPhase.Performed)
        {
            isMoving = true;
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isMoving = false;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public bool IsCursorActive()
    {
        return canLook;
    }

    public void ToggleCursor(bool _toggle)
    {
        Cursor.lockState = _toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !_toggle;
    }
}
