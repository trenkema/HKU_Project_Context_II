using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObject : IInteractable
{
    public override string GetInteractPrompt(InteractableTypes _interactableType, string _interactableName)
    {
        switch (_interactableType)
        {
            case InteractableTypes.Controlable:
                return string.Format("Control {0}", _interactableName);
            case InteractableTypes.Pickupable:
                return string.Format("Pickup {0}", _interactableName);
            case InteractableTypes.Enterable:
                return string.Format("Enter {0}", _interactableName);
        }

        return null;
    }

    public override void OnInteract(InteractableTypes _interactableType)
    {
        throw new System.NotImplementedException();
    }
}
