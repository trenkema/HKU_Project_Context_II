using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RobotArmController : MonoBehaviour
{
    [SerializeField] Transform robotHitPoint;

    [SerializeField] Animator animator;

    [Header("UI Settings")]
    [SerializeField] GameObject hud;

    [SerializeField] Image hitPointer;
    [SerializeField] float pointerMoveTime;
    [SerializeField] float minPointerPosition, maxPointerPosition;

    [Space(10)]

    [SerializeField] float minRedZone;
    [SerializeField] float maxRedZone;
    [SerializeField] float minOrangeZone, maxOrangeZone;
    [SerializeField] float minGreenZone, maxGreenZone;

    [Space(10)]

    [SerializeField] int redZoneHitAmount = 1;
    [SerializeField] int OrangeZoneHitAmount = 2;
    [SerializeField] int GreenZoneHitAmount = 3;

    [Header("Arm Settings")]
    [SerializeField] float rotateSpeed = 5f;
    [SerializeField] float rotateDamping = 5f;

    [Space(10)]

    [SerializeField] float minYRotation;
    [SerializeField] float maxYRotation;

    bool isControlling = false;

    bool canHit = true;

    bool moveRight = true;

    bool hasHit = false;

    Vector3 startPos;

    Vector3 currentValue;

    Vector2 curRotateInput = Vector2.zero;

    float newYRotation;

    float pointerLerpTimeElapsed;

    float amountOfHits = 0;

    private void Start()
    {
        newYRotation = transform.localEulerAngles.y;
    }

    private void Update()
    {
        if (canHit && hud.activeInHierarchy)
        {
            if (pointerLerpTimeElapsed < pointerMoveTime && moveRight)
            {
                currentValue = Vector3.Lerp(startPos, new Vector3(maxPointerPosition, hitPointer.rectTransform.localPosition.y, hitPointer.rectTransform.localPosition.z), pointerLerpTimeElapsed / pointerMoveTime);

                pointerLerpTimeElapsed += Time.deltaTime;
            }

            if (pointerLerpTimeElapsed < pointerMoveTime && !moveRight)
            {
                currentValue = Vector3.Lerp(startPos, new Vector3(minPointerPosition, hitPointer.rectTransform.localPosition.y, hitPointer.rectTransform.localPosition.z), pointerLerpTimeElapsed / pointerMoveTime);

                pointerLerpTimeElapsed += Time.deltaTime;
            }

            if (pointerLerpTimeElapsed >= pointerMoveTime)
            {
                startPos = hitPointer.rectTransform.localPosition;
                pointerLerpTimeElapsed = 0;
                moveRight = !moveRight;
            }

            hitPointer.rectTransform.localPosition = currentValue;
        }
    }

    private void FixedUpdate()
    {
        RotateArm();
    }

    public void EnableHUD()
    {
        hud.SetActive(true);

        startPos = new Vector3(Random.Range(minPointerPosition, maxPointerPosition), hitPointer.rectTransform.localPosition.y, hitPointer.rectTransform.localPosition.z);
        hitPointer.rectTransform.localPosition = startPos;

        pointerLerpTimeElapsed *= Mathf.Abs(startPos.x) / (Mathf.Abs(minPointerPosition) + maxPointerPosition);

        canHit = true;

        hasHit = false;
    }

    public void Rotate(InputAction.CallbackContext _context)
    {
        curRotateInput = _context.ReadValue<Vector2>();
    }

    private void RotateArm()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") && canHit && isControlling)
        {
            newYRotation += rotateSpeed * curRotateInput.x * Time.fixedDeltaTime;

            newYRotation = Mathf.Clamp(newYRotation, minYRotation, maxYRotation);

            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(transform.localEulerAngles.x, newYRotation, transform.localEulerAngles.z), Time.fixedDeltaTime * rotateDamping);
        }
    }

    public void Hit(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") && canHit && isControlling)
            {
                canHit = false;

                float hitPointerPos = Mathf.Abs(hitPointer.rectTransform.localPosition.x);

                if (hitPointerPos > minRedZone && hitPointerPos < maxRedZone)
                {
                    // In Red Zone
                    amountOfHits = redZoneHitAmount;
                }

                if (hitPointerPos > minOrangeZone && hitPointerPos < maxOrangeZone)
                {
                    // In Orange Zone
                    amountOfHits = OrangeZoneHitAmount;
                }

                if (hitPointerPos > minGreenZone && hitPointerPos < maxGreenZone)
                {
                    // In Green Zone
                    amountOfHits = GreenZoneHitAmount;
                }

                animator.SetTrigger("Hit");

                StartCoroutine(DisableHUD());
            }
        }
    }

    public void DealHit()
    {
        if (!canHit && !hasHit)
        {
            Collider[] colliders = Physics.OverlapSphere(robotHitPoint.position, 2f, Physics.AllLayers, QueryTriggerInteraction.Collide);

            foreach (var collider in colliders)
            {
                if (collider.GetComponent<Pilar>() != null)
                {
                    collider.GetComponent<Pilar>().Hit(amountOfHits);

                    hasHit = true;

                    amountOfHits = 0;

                    break;
                }
            }
        }
    }

    IEnumerator DisableHUD()
    {
        yield return new WaitForSeconds(0.25f);

        hud.SetActive(false);
    }

    public void ToggleControl()
    {
        if (canHit)
        {
            isControlling = !isControlling;

            if (isControlling)
                EnableHUD();
            else
                hud.SetActive(false);
        }
    }

    public bool CanToggleControl()
    {
        return canHit;
    }
}
