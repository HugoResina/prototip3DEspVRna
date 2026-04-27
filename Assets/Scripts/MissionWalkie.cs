using UnityEngine;

public class MissionWalkie : MissionGameObject, IIteractable
{
    [SerializeField] private string _interactionPrompt = "Prem 'E' per agafar el walkie";

    public string InteractionPrompt => _interactionPrompt;
    public bool InteractionEnabled => enabled;

    private Collider _collider;
    private GameObject _interactor;

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

    private void Update()
    {
        Camera camera = Camera.main;
        transform.position = camera.transform.position + camera.transform.forward * 0.5f;
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
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
