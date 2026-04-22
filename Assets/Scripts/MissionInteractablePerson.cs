using UnityEngine;

public class MissionInteractablePerson : MissionGameObject, IIteractable
{
    [SerializeField] private string _interactionPrompt = "Prem 'E' per interactuar";

    public string InteractionPrompt => _interactionPrompt;
    public bool InteractionEnabled => enabled;

    private Collider _collider;
    private GameObject _interactor;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        if (_collider == null)
        {
            Debug.LogError("MISSION INTERACTABLE PERSON: Missing a Collider component.");
        }
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerInteraction interaction))
        {
            interaction.isInteracting = true;
            _interactor = interactor;
        }

        UIManager.Instance.SetCursorState(false, true);

        OnDecisionMade();
    }

    public void EndInteraction()
    {
        if (_interactor != null && _interactor.TryGetComponent(out PlayerInteraction interaction))
        {
            interaction.isInteracting = false;
            _interactor = null;
        }

        UIManager.Instance.SetCursorState(true, false);
    }
}
