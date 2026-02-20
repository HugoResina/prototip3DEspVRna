using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public bool InteractablePersonCanvasState { private get => _interactablePersonCanvas.activeSelf; set => _interactablePersonCanvas.SetActive(value); }

    [SerializeField] private GameObject _interactablePersonCanvas;
    [SerializeField] private Text _aiOutput;

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

    public void SetCursorState(bool looked, bool visible)
    {
        Cursor.lockState = looked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = visible;
    }
    public void SetAiOutputText(string output)
    {
        _aiOutput.text = output;
    }
}
