using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactionPrompt = "Prem 'E' per interactuar";
    public Canvas TalkCanvas;

    public virtual void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
        //activate canvas to talk
        TalkCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void TurnOffCanvas()
    {
        TalkCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

}
