using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public abstract bool CanInteract { get; set; }
    public abstract void Interact();
}
