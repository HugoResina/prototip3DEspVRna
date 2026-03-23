using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class ControlsTutorial : MonoBehaviour
{
    [SerializeField] private bool _startOnAwake = true;
    [SerializeField] private PlayerInputs _inputs;
    [SerializeField] private TutorialObjectiveSO[] _objectives;

    public static event Action<string, string> OnUpdateObjective;

    private readonly Dictionary<TutorialObjectiveType, Func<bool>> _conditions = new Dictionary<TutorialObjectiveType, Func<bool>>();

    private bool _waiting = false;

    private void Awake()
    {
        SetObjectivesConditions();

        if (_startOnAwake) StartTutorial();
    }

    private void SetObjectivesConditions()
    {
        for (int i = 0; i < _objectives.Length; i++)
        {
            switch (_objectives[i].tutorialType)
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

                case TutorialObjectiveType.Attack:
                    _conditions.Add(TutorialObjectiveType.Attack, () => _inputs.AttackInput);
                    break;

                case TutorialObjectiveType.Interact:
                    _conditions.Add(TutorialObjectiveType.Interact, () => _inputs.InteractInput);
                    break;

                case TutorialObjectiveType.Exit:
                    _conditions.Add(TutorialObjectiveType.Exit, () => _inputs.ExitInput);
                    break;

                default:
                    Debug.LogError($"CONTROLS TUTORIAL: Objective with type '{_objectives[i].tutorialType}' is not a recognized Type");
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
        foreach (var objective in _objectives)
        {
            OnUpdateObjective?.Invoke(objective.Title, objective.Description);

            StartCoroutine(WaitForKeyPressed(_conditions[objective.tutorialType]));
            yield return new WaitWhile(() => _waiting);
            Debug.Log($"CONTROLS TUTORIAL: {objective.tutorialType} complet");
        }

        OnUpdateObjective?.Invoke(string.Empty, string.Empty);
    }

    private IEnumerator WaitForKeyPressed(Func<bool> condition)
    {
        _waiting = true;
        yield return new WaitUntil(condition);
        _waiting = false;
    }
}
