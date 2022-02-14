using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] Transform pickupHolder;

    [SerializeField] private float checkRate = 0.05f;
    [SerializeField] private float maxCheckDistance;
    [SerializeField] private LayerMask layerMask;
    private float lastCheckTime;

    private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    private GameObject curPickedupInteractGameObject;
    private IInteractable curPickedupInteractable;

    [SerializeField] private TextMeshProUGUI promptPickupText;
    [SerializeField] private TextMeshProUGUI promptControlText;
    [SerializeField] private TextMeshProUGUI promptEnterText;

    [SerializeField] Camera playerCamera;

    private bool isHolding = false;

    private void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject != curInteractGameObject && hit.collider.gameObject != curPickedupInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponentInParent<IInteractable>();

                    promptPickupText.gameObject.SetActive(false);
                    promptControlText.gameObject.SetActive(false);
                    promptEnterText.gameObject.SetActive(false);

                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;

                promptPickupText.gameObject.SetActive(false);
                promptControlText.gameObject.SetActive(false);
                promptEnterText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        foreach (var interactable in curInteractable.interactableTypeSelector)
        {
            switch (interactable.interactableTypes)
            {
                case InteractableTypes.Pickupable:
                    if (curInteractable.GetInteractPrompt(InteractableTypes.Pickupable, "") != null)
                    {
                        promptPickupText.gameObject.SetActive(true);
                        promptPickupText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt(InteractableTypes.Pickupable, interactable.interactableName));
                    }

                    break;
                case InteractableTypes.Controlable:
                    if (curInteractable.GetInteractPrompt(InteractableTypes.Controlable, "") != null)
                    {
                        promptControlText.gameObject.SetActive(true);
                        promptControlText.text = string.Format("<b>[Q]</b> {0}", curInteractable.GetInteractPrompt(InteractableTypes.Controlable, interactable.interactableName));
                    }

                    break;
                case InteractableTypes.Enterable:
                    if (curInteractable.GetInteractPrompt(InteractableTypes.Enterable, "") != null)
                    {
                        promptEnterText.gameObject.SetActive(true);
                        promptEnterText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt(InteractableTypes.Enterable, interactable.interactableName));
                    }

                    break;
            }

        }
    }

    public void OnPickupInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null && !isHolding)
        {
            foreach (var interactable in curInteractable.interactableTypeSelector)
            {
                if (interactable.interactableTypes == InteractableTypes.Pickupable)
                {
                    // Add Object To Inventory

                    promptPickupText.gameObject.SetActive(false);
                }
            }
        }
        else if (context.phase == InputActionPhase.Started && curPickedupInteractable != null && isHolding)
        {
            isHolding = false;

            curPickedupInteractable = null;
            curPickedupInteractGameObject = null;

            // Drop Object
        }
    }

    public void OnControlable(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            foreach (var interactable in curInteractable.interactableTypeSelector)
            {
                if (interactable.interactableTypes == InteractableTypes.Controlable)
                {
                    curInteractable.OnInteract(InteractableTypes.Controlable);

                    promptControlText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnEnterable(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            foreach (var interactable in curInteractable.interactableTypeSelector)
            {
                if (interactable.interactableTypes == InteractableTypes.Enterable)
                {
                    curInteractable.OnInteract(InteractableTypes.Enterable);

                    promptEnterText.gameObject.SetActive(false);
                }
            }
        }
    }
}
