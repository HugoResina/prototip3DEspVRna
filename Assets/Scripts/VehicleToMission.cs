using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleToMission : MonoBehaviour, IIteractable
{
    public string InteractionPrompt { get => _interactionPrompt; set => _interactionPrompt = value; }

    [SerializeField] private string _interactionPrompt = "Prem 'E' per començar la missió";

    public MisionSO mission;


    public void Interact()
    {
        
        UIManager.Instance.InteractablePersonMenuState = false;
        SceneManager.LoadScene(mission.SceneId);
    }

   
}
