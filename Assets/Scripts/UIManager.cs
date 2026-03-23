using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    #region Player HUD SerializeVariables
    [Header("Player HUD")]
    [SerializeField] private TextMeshProUGUI _pInteractionText;
    [SerializeField] private GameObject _objectiveSection;
    [SerializeField] private TextMeshProUGUI _objectiveTitle;
    [SerializeField] private TextMeshProUGUI _objectiveText;
    #endregion

    #region Interactable Person SerializeVariables, Properties and Events
    [Header("Interactable Person References")]
    [SerializeField] private GameObject _interactablePersonMenu;
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private TextMeshProUGUI _ipResponseText;
    [SerializeField] private Button _ipEraseButton;
    [SerializeField] private Button _ipSendButton;
    [SerializeField] private Button _ipMicrophoneToggleButton;
    [SerializeField] private Color _ipMicrophoneRecordingColor = Color.green;
    [SerializeField] private Color _ipMicrophoneStoppedColor = Color.red;

    public string InterPerInputFieldText => _ipInputField.text;

    public static event Action InterPerToggleMicrophone;
    #endregion

    private void OnEnable()
    {
        PlayerInteraction.OnInteractUpdate += UpdateInteractionText;

        ControlsTutorial.OnUpdateObjective += UpdatePlayerObjective;

        InteractablePersonEvents.OnMenuState += UpdateInteractablePersonMenuState;
        InteractablePersonEvents.OnInputField += UpdateInteractablePersonInputField;
        InteractablePersonEvents.OnResponse += UpdateInteractablePersonResponseText;
    }

    private void OnDisable()
    {
        PlayerInteraction.OnInteractUpdate -= UpdateInteractionText;

        ControlsTutorial.OnUpdateObjective -= UpdatePlayerObjective;

        InteractablePersonEvents.OnMenuState -= UpdateInteractablePersonMenuState;
        InteractablePersonEvents.OnInputField -= UpdateInteractablePersonInputField;
        InteractablePersonEvents.OnResponse -= UpdateInteractablePersonResponseText;
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
        if (_interactablePersonMenu.activeSelf)
        {
            _ipMicrophoneToggleButton.gameObject.GetComponent<Image>().color = NewSTT.Recording ? _ipMicrophoneRecordingColor : _ipMicrophoneStoppedColor;
        }
        #endregion
    }

    private void UpdateInteractionText(string text) => _pInteractionText.text = text;

    public void SetCursorState(bool looked, bool visible)
    {
        Cursor.lockState = looked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = visible;
    }

    private void UpdatePlayerObjective(string title, string objective)
    {
        _objectiveTitle.text = title;
        _objectiveText.text = objective;
    }

    #region Interactable Person Functions
    private void UpdateInteractablePersonMenuState(bool state) => _interactablePersonMenu.SetActive(state);
    private void UpdateInteractablePersonInputField(string text) => _ipInputField.text = text;
    private void UpdateInteractablePersonResponseText(string text) => _ipResponseText.text = text;

    #region Button Functions
    private void InterPerEraseButton()
    {
        _ipInputField.text = "";
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
    #endregion
}
