using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsTutorial : MonoBehaviour
{
    [SerializeField] private bool _startOnAwake = true;
    [SerializeField] private float _startWaitTime = 1f;
    [SerializeField] private PlayerInputs _inputs;
    [SerializeField] private TutorialObjectivesSO _tutorialObjectives;
    [SerializeField] private float _popupWaitTime = 2f;

    public static event Action<string, string> OnUpdateObjective;
    public static event Action<PopupSO, Action> OnShowPopup;

    private readonly Dictionary<TutorialObjectiveType, Func<bool>> _conditions = new Dictionary<TutorialObjectiveType, Func<bool>>();

    private bool _waiting = false;
    private bool _grabbing = false;
    private bool _relese = false;

    private void OnEnable() => GrabbableBehaviour.OnGrabItem += UpdateGrabbing;
    private void OnDisable() => GrabbableBehaviour.OnGrabItem -= UpdateGrabbing;

    private void Awake()
    {
        SetObjectivesConditions();
    }

    private void Start()
    {
        if (_startOnAwake) StartTutorial();
    }

    private void SetObjectivesConditions()
    {
        for (int i = 0; i < _tutorialObjectives.objectives.Length; i++)
        {
            switch (_tutorialObjectives.objectives[i].tutorialType)
            {
                case TutorialObjectiveType.Look:
                    _conditions.Add(TutorialObjectiveType.Look, () => _inputs.LookInput.sqrMagnitude > 0);
                    break;

                case TutorialObjectiveType.Move:
                    _conditions.Add(TutorialObjectiveType.Move, () => _inputs.MoveInput.sqrMagnitude > 0);
                    break;

                case TutorialObjectiveType.Sprint:
                    _conditions.Add(TutorialObjectiveType.Sprint, () => _inputs.MoveInput.sqrMagnitude > 0 && _inputs.SprintInput);
                    break;

                case TutorialObjectiveType.Pickup:
                    _conditions.Add(TutorialObjectiveType.Pickup, () => (_inputs.InteractInput) && _grabbing);
                    break;

                case TutorialObjectiveType.Rotate:
                    _conditions.Add(TutorialObjectiveType.Rotate, () => (_inputs.AttackInput && _inputs.LookInput.sqrMagnitude > 0) && _grabbing);
                    break;

                case TutorialObjectiveType.Relese:
                    _conditions.Add(TutorialObjectiveType.Relese, () => (_inputs.ExitInput || _inputs.InteractInput) && _relese);
                    break;

                default:
                    Debug.LogError($"CONTROLS TUTORIAL: Objective with type '{_tutorialObjectives.objectives[i].tutorialType}' is not a recognized Type");
                    break;
            }
        }
    }

    public void StartTutorial()
    {
        StartCoroutine(Tutorial());
    }
    
    private IEnumerator Tutorial()
    {
        bool firstPopup = true;

        yield return new WaitForSeconds(_startWaitTime);

        foreach (var objective in _tutorialObjectives.objectives)
        {
            if (objective.popup != null)
            {
                if (!firstPopup) yield return new WaitForSeconds(_popupWaitTime);
                
                _waiting = true;
                OnShowPopup?.Invoke(objective.popup, () => _waiting = false);
                yield return new WaitWhile(() => _waiting);
            }

            OnUpdateObjective?.Invoke(objective.title, objective.description);

            StartCoroutine(WaitForKeyPressed(_conditions[objective.tutorialType]));
            yield return new WaitWhile(() => _waiting);
            Debug.Log($"CONTROLS TUTORIAL: {objective.tutorialType} complet");

            firstPopup = false;
        }
    }

    private IEnumerator WaitForKeyPressed(Func<bool> condition)
    {
        _waiting = true;
        yield return new WaitUntil(condition);
        _waiting = false;

        OnUpdateObjective?.Invoke(string.Empty, string.Empty);
    }

    private void UpdateGrabbing(bool grabbing)
    {
        _grabbing = grabbing;

        if (!grabbing) StartCoroutine(ResetRelese());
    }

    private IEnumerator ResetRelese()
    {
        _relese = true;
        yield return new WaitForSeconds(1f);
        _relese = false;
    }
}
