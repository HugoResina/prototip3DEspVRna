using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactionPrompt = "Prem 'E' per interactuar";
    public Canvas TalkCanvas;
    public static event Action<bool> lockCam;

    private void OnEnable()
    {
        PlayerInputs.ExitFunc += TurnOffCanvas;
    }
    private void OnDisable()
    {
        PlayerInputs.ExitFunc -= TurnOffCanvas;
    }
    

    public virtual void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
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
