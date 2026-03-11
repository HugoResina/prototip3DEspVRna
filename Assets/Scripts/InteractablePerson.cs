using System;
using UnityEngine;

public class InteractablePerson : MonoBehaviour, IIteractable
{
    public string InteractionPrompt { get => _interactionPrompt; }

    [SerializeField] private string _interactionPrompt = "Prem 'E' per interactuar";

    public PromptBaseSO prompt;

    private void OnEnable()
    {
        PlayerInputs.ExitFunc += TurnOffCanvas;

    }
    private void OnDisable()
    {
        PlayerInputs.ExitFunc -= TurnOffCanvas;
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerInteraction pi))
            pi.isInteracting = true;

        InteractablePersonEvents.UpdateMenuState(true);
        InteractablePersonEvents.UpdateResponseText(string.Empty);

        UIManager.Instance.SetCursorState(false, true);
        GameManager.Instance.SttEnabled = true;
    }
    public void TurnOffCanvas()
    {
        InteractablePersonEvents.UpdateMenuState(false);

        UIManager.Instance.SetCursorState(true, false);
        GameManager.Instance.SttEnabled = false;
    }
}
