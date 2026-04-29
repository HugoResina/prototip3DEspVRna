using UnityEngine;

public class MissionInteractablePerson : MissionGameObject, IIteractable
{
    // Promp per l'interacció
    [SerializeField] private string _interactionPrompt = "Prem 'E' per interactuar";

    // Propietats d'interacció heretades de IIteractable
    public string InteractionPrompt => _interactionPrompt;
    public bool InteractionEnabled => enabled;

    private Collider _collider; // Collider per la interacció
    private GameObject _interactor; // Actual interactuador

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        if (_collider == null)
        {
            Debug.LogError("MISSION INTERACTABLE PERSON: Missing a Collider component.");
        }
    }

    #region Interaction Functions
    /// <summary>
    /// En interactuar es pren la decisió de interactuar
    /// </summary>
    /// <param name="interactor">Interactuador que ha interactuat</param>
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

    /// <summary>
    /// Finalitza l'interacció
    /// </summary>
    public void EndInteraction()
    {
        if (_interactor != null && _interactor.TryGetComponent(out PlayerInteraction interaction))
        {
            interaction.isInteracting = false;
            _interactor = null;
        }

        UIManager.Instance.SetCursorState(true, false);
    }
    #endregion
}
