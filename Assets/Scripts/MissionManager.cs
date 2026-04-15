using System.Collections;
using System.Linq;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    private DialogueHandler _dialogueHandler;
    private FreeRoamHandler _freeRoamHandler;

    private MissionData _currentMission;
    private MissionStep _currentStep;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _dialogueHandler = GetComponent<DialogueHandler>();
        _freeRoamHandler = GetComponent<FreeRoamHandler>();
    }

    private void Start()
    {
        StartCoroutine(StartMissionAfterDelay("M01_FuitaAmoniac", 2f));
    }

    private IEnumerator StartMissionAfterDelay(string missionId, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadMission(missionId);
    }

    public void LoadMission(string missionId)
    {
        _currentMission = MissionLoader.Load(missionId); // llegeix el JSON
        GoToStep(_currentMission.entry);
    }

    public void GoToStep(string stepId)
    {
        _currentStep = _currentMission.steps.FirstOrDefault(s => s.id == stepId);

        // Filtra les decisions per flags (mostra només les disponibles)
        var validDecisions = _currentStep.decisions
            .Where(d => d.requires == null || d.requires.All(req => FlagSystem.Instance.GetFlag(req.flag) == (req.value == "true")))
            .ToList();

        // Avisa la UI que mostri les opcions
        //DialogueUI.Instance.ShowStep(_currentStep.description, validDecisions);
        Debug.Log($"Step: {_currentStep.id} - { _currentStep.objectiveText}");

        switch (_currentStep.type)
        {
            case "dialogue":
                _dialogueHandler.StartDialogue(_currentStep);
                break;

            case "freeroam":
                _freeRoamHandler.ActivateForStep(_currentStep);
                break;

            default:
                Debug.LogWarning($"MISSION MANAGER: Unknown step type: {_currentStep.type}");
                break;
        }
    }

    public void OnDecisionMade(Decision decision)
    {
        // 1. Aplica el flag de la decisió
        if (decision.effects != null)
        {
            foreach (var effect in decision.effects)
            {
                FlagSystem.Instance.SetFlag(effect.flag, true);
            }
        }

        // 2. Executa lògica específica si cal (via codi)
        DecisionLogic.Execute(decision.id);

        // 3. Avança al proper pas
        GoToStep(decision.next);
    }

    public void RegisterFreeRoamGameObject(string id, MissionGameObject obj)
    {
        _freeRoamHandler.RegisterGameObject(id, obj);
    }
}