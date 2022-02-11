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

    bool isControlling = false;

    bool canHit = true;

    bool moveRight = true;

    bool hasHit = false;

    Vector3 startPos;

    Vector3 currentValue;

    float pointerLerpTimeElapsed;

    int amountOfHits = 0;

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

    public void EnableHUD()
    {
        hud.SetActive(true);

        startPos = new Vector3(Random.Range(minPointerPosition, maxPointerPosition), hitPointer.rectTransform.localPosition.y, hitPointer.rectTransform.localPosition.z);
        hitPointer.rectTransform.localPosition = startPos;

        pointerLerpTimeElapsed *= Mathf.Abs(startPos.x) / (Mathf.Abs(minPointerPosition) + maxPointerPosition);

        canHit = true;

        hasHit = false;
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
                    Debug.Log("Hit Red Zone");

                    amountOfHits = redZoneHitAmount;
                }

                if (hitPointerPos > minOrangeZone && hitPointerPos < maxOrangeZone)
                {
                    // In Orange Zone
                    Debug.Log("Hit Orange Zone");

                    amountOfHits = OrangeZoneHitAmount;
                }

                if (hitPointerPos > minGreenZone && hitPointerPos < maxGreenZone)
                {
                    // In Green Zone
                    Debug.Log("Hit Green Zone");

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

                    Debug.Log("Hit Pilar");

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
