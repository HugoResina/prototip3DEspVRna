using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactionPrompt = "Prem 'E' per interactuar";

    public virtual void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }

}
