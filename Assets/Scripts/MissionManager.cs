using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DialogueHandler), typeof(FreeRoamHandler))] // Requereix els handlers da cada tipus per funcionar
public class MissionManager : MonoBehaviour
{
    // Instància estàtica
    public static MissionManager Instance { get; private set; }

    // Event per actualitzar la UI
    public static event Action<string, string> OnUpdateObjective;

    // MonoBehaviours dels tipus de decisions
    private DialogueHandler _dialogueHandler;
    private FreeRoamHandler _freeRoamHandler;

    // Missió i pas actual
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

    // Subscripció i desubscripció d'events
    private void OnEnable() => ExitCInematic.StartMission += StartMision;
    private void OnDisable() => ExitCInematic.StartMission -= StartMision;

    #region Mission Loading
    #region Testing Corroutine
    /// <summary>
    /// Corrutina per carregar i iniciar la missió amb un temps d'espera
    /// </summary>
    /// <param name="missionId">Identificador de la missió a carregar</param>
    /// <param name="delay">Temps d'espera</param>
    private IEnumerator StartMissionAfterDelay(string missionId, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadMission(missionId);
    }
    #endregion

    /// <summary>
    /// Carrega i inicia una missió
    /// </summary>
    public void StartMision()
    {
        LoadMission("M01_FuitaAmoniac");
    }

    /// <summary>
    /// Carrega una missió i la inicia
    /// </summary>
    /// <param name="missionId">Identificador de la missió a carregar</param>
    public void LoadMission(string missionId)
    {
        _currentMission = MissionLoader.Load(missionId);
        GoToStep(_currentMission.entry);
    }
    #endregion

    #region Step Handling
    /// <summary>
    /// Comprova el pas següent i l'inicia segons el tipus que és
    /// </summary>
    /// <param name="stepId">Identificador del pas</param>
    public void GoToStep(string stepId)
    {
        _currentStep = _currentMission.steps.FirstOrDefault(s => s.id == stepId);

        if (_currentStep == null)
        {
            Debug.LogError($"MISSION MANAGER: Step with id '{stepId}' not found in the mission.");
            return;
        }

        // Avisa la UI que mostri l'objectiu
        OnUpdateObjective?.Invoke(_currentStep.objectiveTitle, _currentStep.objectiveText);

        // Comprova el tipus de pas i l'inicia en conseqüència
        switch (_currentStep.type)
        {
            case "dialogue":
                HandleDialogueStep(_currentStep);
                break;

            case "freeroam":
                HandleFreeRoamStep(_currentStep);
                break;

            default:
                Debug.LogWarning($"MISSION MANAGER: Unknown step type: {_currentStep.type}");
                break;
        }
    }

    /// <summary>
    /// Inicia un pas de tipus FreeRoam
    /// </summary>
    /// <param name="step">Identificador del pas</param>
    private void HandleFreeRoamStep(MissionStep step)
    {
        var validDecisions = step.decisions?
            .Where(d => d.requires == null ||
                        d.requires.All(req => FlagSystem.Instance.GetFlag(req.flag) == req.value))
            .ToList();

        _freeRoamHandler.ActivateForStep(_currentStep);
    }

    /// <summary>
    /// Inicia un pas de tipus Dialogue
    /// </summary>
    /// <param name="step">Identificador del pas</param>
    private void HandleDialogueStep(MissionStep step)
    {
        _dialogueHandler.StartDialogue(step);
    }

    /// <summary>
    /// Executa tot el necessari en realitzar una decisió, així com passar a la següent decisió.
    /// </summary>
    /// <param name="decision">Decisió que s'ha pres</param>
    public void OnDecisionMade(Decision decision)
    {
        if (_currentMission.steps.FirstOrDefault(s => s.id == decision.next).id == _currentStep.id) return;

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
    #endregion

    /// <summary>
    /// Registra un MissionGameObject en FreeRoamHandler
    /// </summary>
    /// <param name="id">Indentificador del nou gameobject registrat</param>
    /// <param name="obj">Objecte a registrar</param>
    public void RegisterFreeRoamGameObject(string id, MissionGameObject obj)
    {
        _freeRoamHandler.RegisterGameObject(id, obj);
    }
}