using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleToMission : MonoBehaviour, IIteractable
{
    public string InteractionPrompt { get => _interactionPrompt; set => _interactionPrompt = value; }
    public bool InteractionEnabled => enabled;

    [SerializeField] private string _interactionPrompt = "Prem 'E' per començar la missió";

    public MisionSO mission;


    public void Interact(GameObject gameObject)
    {
        InteractablePersonEvents.UpdateMenuState(false);
        SceneManager.LoadScene(mission.SceneId);
    }
}
