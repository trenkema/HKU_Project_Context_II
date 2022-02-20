using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableTypes { Controlable, Pickupable, Enterable }

public abstract class IInteractable : MonoBehaviour
{
    public List<InteractableTypeSelector> interactableTypeSelector = new List<InteractableTypeSelector>();

    public abstract string GetInteractPrompt(InteractableTypes _interactableType, string _interactableName);
    public abstract void OnInteract(InteractableTypes _interactableType);
}

[System.Serializable]
public class InteractableTypeSelector
{
    public InteractableTypes interactableTypes;
    public string interactableName;
}