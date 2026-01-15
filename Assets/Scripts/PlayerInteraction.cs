using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _playerCamera;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _interactiontext;


    [Header("Settings")]
    public float interactDistance = 2f;

    private PlayerInputs _playerInputs;

    private Interactable lastInteracted;

    private void Awake()
    {
        _playerInputs = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        CheckInteraction();
        //checkExit();
       
    }

    
    private void CheckInteraction()
    {
        RaycastHit hit;

        Vector3 rayOrigin = _playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

       

        if (Physics.Raycast(rayOrigin, _playerCamera.transform.forward, out hit, interactDistance))
        {
            
            
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            //Debug.Log(interactable);
            if (interactable != null)
            {
                lastInteracted = interactable;
                _interactiontext.text = interactable.interactionPrompt;

                if (_playerInputs.InteractInput)
                {
                    interactable.Interact();
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
            if(lastInteracted != null)
            {
                lastInteracted.TurnOffCanvas();
                lastInteracted = null;
            }
            
        }
    }
    //private void checkExit()
    //{

    //}
}
