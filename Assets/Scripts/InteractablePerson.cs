using System;
using UnityEngine;

public class InteractablePerson : MonoBehaviour, IIteractable
{
    public string InteractionPrompt { get => _interactionPrompt; }

    [SerializeField] private string _interactionPrompt = "Prem 'E' per interactuar";

    public PromptBaseSO prompt;

    public static event Action<bool> OnInteract;

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

        OnInteract?.Invoke(true);

        UIManager.Instance.InterPerResponseText = string.Empty;
        //UIManager.Instance.InteractablePersonMenuState = true;
        UIManager.Instance.SetCursorState(false, true);
        GameManager.Instance.SttEnabled = true;
    }
    public void TurnOffCanvas()
    {
        OnInteract?.Invoke(false);

        //UIManager.Instance.InteractablePersonMenuState = false;
        UIManager.Instance.SetCursorState(true, false);
        GameManager.Instance.SttEnabled = false;
    }
}
