using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player HUD")]
    [SerializeField] private TextMeshProUGUI _pInteractionText;

    #region Interactable Person Properties, SerializeVariables and Events
    public bool InteractablePersonMenuState { private get => _interactablePersonMenu.activeSelf; set => _interactablePersonMenu.SetActive(value); }
    public string InterPerInputFieldText { get => _ipInputField.text; set => _ipInputField.text = value; }
    public string InterPerResponseText { get => _ipResponseText.text; set => _ipResponseText.text = value; }

    [Header("Interactable Person References")]
    [SerializeField] private GameObject _interactablePersonMenu;
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private TextMeshProUGUI _ipResponseText;
    [SerializeField] private Button _ipEraseButton;
    [SerializeField] private Button _ipSendButton;
    [SerializeField] private Button _ipMicrophoneToggleButton;
    [SerializeField] private Color _ipMicrophoneRecordingColor = Color.green;
    [SerializeField] private Color _ipMicrophoneStoppedColor = Color.red;

    public static event Action InterPerToggleMicrophone;
    #endregion

    private void OnEnable()
    {
        PlayerInteraction.OnInteractUpdate += UpdateInteractionText;

        InteractablePerson.OnInteract += UpdateInteractablePersonMenuState;
    }

    private void OnDisable()
    {
        PlayerInteraction.OnInteractUpdate -= UpdateInteractionText;

        InteractablePerson.OnInteract += UpdateInteractablePersonMenuState;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        #region Interactable Person Button Listeners
        _ipEraseButton.onClick.AddListener(InterPerEraseButton);
        _ipSendButton.onClick.AddListener(InterPerSendButton);
        _ipMicrophoneToggleButton.onClick.AddListener(ToggleMicrophone);
        #endregion
    }

    private void Update()
    {
        #region Ineractable Person Microphone Button Record Color
        if (InteractablePersonMenuState)
        {
            _ipMicrophoneToggleButton.gameObject.GetComponent<Image>().color = NewSTT.Recording ? _ipMicrophoneRecordingColor : _ipMicrophoneStoppedColor;
        }
        #endregion
    }

    private void UpdateInteractionText(string text) => _pInteractionText.text = text;
    private void UpdateInteractablePersonMenuState(bool state) => _interactablePersonMenu.SetActive(state);

    public void SetCursorState(bool looked, bool visible)
    {
        Cursor.lockState = looked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = visible;
    }

    #region Interactable Person Button Functions
    private void InterPerEraseButton()
    {
        InterPerInputFieldText = "";
    }

    private void InterPerSendButton()
    {
        GameManager.Instance.SendFunc();
    }

    private void ToggleMicrophone()
    {
        InterPerToggleMicrophone?.Invoke();
    }
    #endregion
}
