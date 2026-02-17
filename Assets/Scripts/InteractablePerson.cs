using System;
using UnityEngine;

public class InteractablePerson : MonoBehaviour, IIteractable
{
    public string InteractionPrompt { get => _interactionPrompt; set => _interactionPrompt = value; }

    [SerializeField] private string _interactionPrompt = "Prem 'E' per interactuar";

    public Canvas TalkCanvas;
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
    

    public void Interact()
    {
        
        TalkCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    
        lockCam?.Invoke(true);
    }
    public void TurnOffCanvas()
    {
        TalkCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lockCam?.Invoke(false);

    }

}
