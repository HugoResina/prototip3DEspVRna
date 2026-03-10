using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _playerCamera;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _interactiontext;

    [Header("Settings")]
    [SerializeField] private float _interactDistance = 2f;
    [SerializeField] private Transform _itemViewer;

    public Vector3 ItemViewerPosition => _itemViewer.position;

    [HideInInspector] public bool isInteracting = false;

    //public static event Action<bool> Interacting;
    public static event Action<string> GetPrompt;

    private PlayerInputs _playerInputs;

    private IGrabbable _currentGrabbable;
    private InteractablePerson _currentInterPerson;

    private void OnEnable() => PlayerInputs.ExitFunc += OnExit;
    private void OnDisable() => PlayerInputs.ExitFunc -= OnExit;
    private void OnExit() => isInteracting = false;

    private void Awake()
    {
        _playerInputs = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        CheckInteraction();
    }
    
    private void CheckInteraction()
    {
        if (isInteracting && _currentGrabbable != null)
        {
            if (_playerInputs.AttackInput)
            {
                _currentGrabbable.RotateItem(_playerCamera, _playerInputs.LookInput.x, _playerInputs.LookInput.y);
            }
        }

        Vector3 rayOrigin = _playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(rayOrigin, _playerCamera.transform.forward, out RaycastHit hit, _interactDistance))
        {
            IIteractable interactable = hit.collider.GetComponent<IIteractable>();
            _currentGrabbable = hit.collider.GetComponent<IGrabbable>();

            if (interactable != null)
            {
                if (interactable is InteractablePerson person)
                {
                    _currentInterPerson = person;
                }

                if (!isInteracting)
                {
                    _interactiontext.text = interactable.InteractionPrompt;
                }
                else if (_currentGrabbable != null)
                {
                    _interactiontext.text = _currentGrabbable.RelesePrompt;
                }

                if (_playerInputs.InteractInput)
                {
                    if (interactable is InteractablePerson) GetPrompt?.Invoke(_currentInterPerson.prompt.Prompt);
                    interactable.Interact(gameObject);
                }
            }
            else
            {
                _interactiontext.text = string.Empty;
            }
        }
        else
        {
            _interactiontext.text = string.Empty;
            if(_currentInterPerson != null)
            {
                _currentInterPerson.TurnOffCanvas();
                _currentInterPerson = null;
            }
            
        }
    }
}
