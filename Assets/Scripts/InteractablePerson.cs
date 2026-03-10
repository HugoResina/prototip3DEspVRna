using System;
using UnityEngine;

public class InteractablePerson : MonoBehaviour, IIteractable
{
    public string InteractionPrompt { get => _interactionPrompt; }

    [SerializeField] private string _interactionPrompt = "Prem 'E' per interactuar";

    public static event Action<bool> lockCam;
    public PromptBaseSO prompt;

    private void Start()
    {
        //prompt = GetComponent<PromptBaseSO>();
    }
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
        UIManager.Instance.InterPerResponseText = string.Empty;
        UIManager.Instance.InteractablePersonMenuState = true;
        UIManager.Instance.SetCursorState(false, true);
        GameManager.Instance.SttEnabled = true;

        lockCam?.Invoke(true);
    }
    public void TurnOffCanvas()
    {
        UIManager.Instance.InteractablePersonMenuState = false;
        UIManager.Instance.SetCursorState(true, false);
        GameManager.Instance.SttEnabled = false;

        lockCam?.Invoke(false);
    }
}
