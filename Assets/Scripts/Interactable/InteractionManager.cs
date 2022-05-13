using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] Transform pickupHolder;

    [SerializeField] private float maxCheckDistance;
    [SerializeField] private LayerMask layerMask;

    //private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    private GameObject curPickedupInteractGameObject;
    private IInteractable curPickedupInteractable;

    [SerializeField] private TextMeshProUGUI promptPickupText;
    [SerializeField] private TextMeshProUGUI promptControlText;
    [SerializeField] private TextMeshProUGUI promptEnterText;

    [SerializeField] Camera playerCamera;

    private bool isHolding = false;

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
