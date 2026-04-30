using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    #region Player HUD SerializeVariables
    [Header("Player HUD")]
    [SerializeField] private TextMeshProUGUI _pInteractionText;

    #region Objective Section
    [SerializeField] private GameObject _objectiveSection;
    [SerializeField] private TextMeshProUGUI _objectiveTitle;
    [SerializeField] private TextMeshProUGUI _objectiveText;

    [SerializeField] private Button _objectiveAnswerButtonPrefab;
    [SerializeField] private GameObject _objectiveAnswerButtonContainer;
    [SerializeField] private TextMeshProUGUI _objectiveScoreText;

    private Stack<Button> _objectiveAnswerButtonStack = new Stack<Button>();
    #endregion
    #endregion

    #region UI Popup SerializeVariables
    [Header("Popup")]
    [SerializeField] private GameObject _popupSection;
    [SerializeField] private TextMeshProUGUI _popupTitle;
    [SerializeField] private Image _popupImage;
    [SerializeField] private TextMeshProUGUI _popupContent;
    [SerializeField] private Button _popupButton;
    #endregion

    #region Interactable Person SerializeVariables, Properties and Events
    [Header("Interactable Person References")]
    [SerializeField] private GameObject _interactablePersonMenu;
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private TextMeshProUGUI _ipResponseText;
    [SerializeField] private Button _ipEraseButton;
    [SerializeField] private Button _ipSendButton;
    [SerializeField] private Button _ipMicrophoneToggleButton;
    [SerializeField] private Image _ipMicrophoneToggleButtonImage;
    [SerializeField] private Color _ipMicrophoneRecordingColor = Color.green;
    [SerializeField] private Sprite _ipMicrophoneRecordingIcon;
    [SerializeField] private Color _ipMicrophoneStoppedColor = Color.red;
    [SerializeField] private Sprite _ipMicrophoneStoppedIcon;

    public string InterPerInputFieldText => _ipInputField.text;

    public static event Action InterPerToggleMicrophone;
    #endregion

    #region Mission Interactable Person (_mip) SerializeVariables
    [Header("Mission Interactable Person References")]
    [SerializeField] private GameObject _mInteractablePersonMenu;
    [SerializeField] private TextMeshProUGUI _mipDialogueText;
    [SerializeField] private GameObject _mipAnswerButtonContainer;
    [SerializeField] private Button _mipAnswerButtonPrefab;

    private static Stack<Button> _mipAnswerButtonStack = new Stack<Button>();
    #endregion

    public event Action<bool> OnPauseGame;

    private void OnEnable()
    {
        PlayerInteraction.OnInteractUpdate += UpdateInteractionText;

        MissionManager.OnUpdateObjective += UpdatePlayerObjective;

        DialogueHandler.OnToggleDialogueMenu += UpdateMissionInterPerMenu;
        DialogueHandler.OnUpdateDialogueText += UpdateMissionInterPerDialogueText;
        DialogueHandler.OnShowDecision += AddMissionInterPerAnswerButton;

        ControlsTutorial.OnUpdateObjective += UpdatePlayerObjective;
        ControlsTutorial.OnShowPopup += ShowPopup;

        InteractablePersonEvents.OnMenuState += UpdateInteractablePersonMenuState;
        InteractablePersonEvents.OnInputField += UpdateInteractablePersonInputField;
        InteractablePersonEvents.OnResponse += UpdateInteractablePersonResponseText;
    }

    private void OnDisable()
    {
        PlayerInteraction.OnInteractUpdate -= UpdateInteractionText;

        MissionManager.OnUpdateObjective -= UpdatePlayerObjective;

        DialogueHandler.OnToggleDialogueMenu -= UpdateMissionInterPerMenu;
        DialogueHandler.OnUpdateDialogueText -= UpdateMissionInterPerDialogueText;
        DialogueHandler.OnShowDecision -= AddMissionInterPerAnswerButton;

        ControlsTutorial.OnUpdateObjective -= UpdatePlayerObjective;
        ControlsTutorial.OnShowPopup -= ShowPopup;

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

        _objectiveSection.SetActive(false);
        _popupSection.SetActive(false);
    }

    private void Update()
    {
        #region Ineractable Person Microphone Button Record Color
        if (_interactablePersonMenu.activeSelf)
        {
            _ipMicrophoneToggleButtonImage.sprite = STT.Recording ? _ipMicrophoneRecordingIcon : _ipMicrophoneStoppedIcon;
            _ipMicrophoneToggleButton.gameObject.GetComponent<Image>().color = STT.Recording ? _ipMicrophoneRecordingColor : _ipMicrophoneStoppedColor;
        }
        #endregion
    }

    private void UpdateInteractionText(string text) => _pInteractionText.text = text;

    private void UpdatePlayerObjective(string title, string objective)
    {
        if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(objective))
        {
            _objectiveSection.SetActive(false);
        }
        else
        {
            _objectiveSection.SetActive(true);
            _objectiveTitle.text = title;
            _objectiveText.text = objective;
        }
    }

    private void AddPlayerAnswerButton(string answerText, Action onClick)
    {
        _objectiveSection.SetActive(true);
        ToggleObjectiveSectionContent(false);

        Button button;
        if (_objectiveAnswerButtonStack.Count > 0)
        {
            button = _objectiveAnswerButtonStack.Pop();
            button.gameObject.SetActive(true);
        }
        else
        {
            button = Instantiate(_objectiveAnswerButtonPrefab, _objectiveAnswerButtonContainer.transform);
        }
        button.GetComponentInChildren<TextMeshProUGUI>().text = answerText;
        button.onClick.AddListener(() =>
        {
            onClick.Invoke();
            RemovePlayerAnswerButtons();
        });
    }

    private void RemovePlayerAnswerButtons()
    {
        foreach (Button button in _objectiveAnswerButtonContainer.GetComponentsInChildren<Button>())
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
            _objectiveAnswerButtonStack.Push(button);
        }
        _objectiveSection.SetActive(false);
        ToggleObjectiveSectionContent(true);
    }

    private void UpdatePlayerObjectiveScore(int score)
    {
        _objectiveScoreText.text = $"Puntuaci�: {score}";
    }

    private void ToggleObjectiveSectionContent(bool showText)
    {
        if (showText)
        {
            _objectiveText.gameObject.SetActive(true);
            _objectiveAnswerButtonContainer.SetActive(false);
        }
        else
        {
            _objectiveText.gameObject.SetActive(false);
            _objectiveAnswerButtonContainer.SetActive(true);
        }
    }

    private void ShowPopup(PopupSO popup, Action onContinue)
    {
        if (popup == null)
        {
            _popupSection.SetActive(false);
        }
        else
        {
            PauseGame();

            _popupSection.SetActive(true);
            _popupTitle.text = popup.title;

            if (popup.image == null) _popupImage.enabled = false;
            else { _popupImage.enabled = true; _popupImage = popup.image; }

            _popupContent.text = popup.content;
            _popupButton.onClick.AddListener(() =>
            {
                _popupSection.SetActive(false);
                ResumeGame();

                onContinue.Invoke();
            });
        }
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

    #region Mission Interactable Person Functions
    private void UpdateMissionInterPerMenu(bool active) => _mInteractablePersonMenu.SetActive(active);
    private void UpdateMissionInterPerDialogueText(string text) => _mipDialogueText.text = text;

    private void AddMissionInterPerAnswerButton(string answerText, Action onClick)
    {
        Button button;
        if (_mipAnswerButtonStack.Count > 0)
        {
            button = _mipAnswerButtonStack.Pop();
            button.gameObject.SetActive(true);
        }
        else
        {
            button = Instantiate(_mipAnswerButtonPrefab, _mipAnswerButtonContainer.transform);
        }

        button.GetComponentInChildren<TextMeshProUGUI>().text = answerText;
        button.onClick.AddListener(() =>
        {
            onClick.Invoke();
            RemoveAllMissionInterPerAnswerButtons();
        });
    }

    private void RemoveAllMissionInterPerAnswerButtons()
    {
        foreach (Button button in _mipAnswerButtonContainer.GetComponentsInChildren<Button>())
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
            _mipAnswerButtonStack.Push(button);
        }
    }
    #endregion

    public void SetCursorState(bool looked, bool visible)
    {
        Cursor.lockState = looked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = visible;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        OnPauseGame?.Invoke(true);

        SetCursorState(false, true);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        OnPauseGame?.Invoke(false);

        SetCursorState(true, false);
    }
}
