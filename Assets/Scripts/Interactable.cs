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
    }

}
