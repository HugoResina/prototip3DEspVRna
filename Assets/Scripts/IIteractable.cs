using UnityEngine;

public interface IIteractable
{
    public string InteractionPrompt { get; set; }

    public void Interact();
}
