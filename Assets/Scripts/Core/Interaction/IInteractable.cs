using UnityEngine;

public interface IInteractable
{
    string GetPrompt(Transform interactor);
    bool CanInteract(Transform interactor);
    void Interact(Transform interactor);
}
