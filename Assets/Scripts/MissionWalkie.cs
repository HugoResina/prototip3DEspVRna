using UnityEngine;

public class MissionWalkie : MissionGameObject, IIteractable
{
    // Promp per l'interacció
    [SerializeField] private string _interactionPrompt = "Prem 'E' per agafar el walkie";

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
            Debug.LogError("MISSION WALKIE: Missing a Collider component.");
            return;
        }

        _collider.excludeLayers = LayerMask.GetMask("Player");
    }

    /// <summary>
    /// Sempre es queda davant de la càmera per a que es pugui interactuar amb ell
    /// </summary>
    private void Update()
    {
        Camera camera = Camera.main;
        transform.position = camera.transform.position + camera.transform.forward * 0.5f;
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
    }

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
}
