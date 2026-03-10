using UnityEngine;

public interface IIteractable
{
    public string InteractionPrompt { get; }

    public void Interact(GameObject interactor);
}
