using UnityEngine;

public interface IIteractable
{
    public string InteractionPrompt { get; }
    public bool InteractionEnabled { get; }

    public void Interact(GameObject interactor);
}
