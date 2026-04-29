using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    #region UI Events
    // Events per actualitzar la UI
    public static event Action<bool> OnToggleDialogueMenu;
    public static event Action<string> OnUpdateDialogueText;
    public static event Action<string, Action> OnShowDecision;
    #endregion

    // Pass actual en el que es troba el jugador
    private MissionStep _currentStep;

    private void Awake()
    {
        OnToggleDialogueMenu?.Invoke(false); // Desactiva el menú de diàleg en per si per algún cas està activat
    }

    #region Dialogue Handling
    /// <summary>
    /// Comença el diàleg. Activa el menú, guarda el pass actual i mostrà el primer diàleg de la conversa
    /// </summary>
    /// <param name="step">Pass al qual pertany el diàleg</param>
    public void StartDialogue(MissionStep step)
    {
        OnToggleDialogueMenu?.Invoke(true);

        _currentStep = step;

        // Mostra el primer exchange
        ShowExchange(step.exchanges[0]);
    }

    /// <summary>
    /// Llança l'event corresponent per mostrar el text del diàleg i llista les decisions disponobles i bloquejades
    /// </summary>
    /// <param name="exchange">Diàleg a mostrar</param>
    private void ShowExchange(Exchange exchange)
    {
        OnUpdateDialogueText?.Invoke(exchange.text);

        // Filtra les decisions en dos grups
        var available = new List<Decision>();
        var locked = new List<Decision>();

        foreach (var d in exchange.decisions)
        {
            // Amaga les ordres ja donades (flag ja és true)
            if (IsAlreadyDone(d)) continue;

            // Separa les disponibles de les bloquejades
            if (MeetsRequirements(d))
                available.Add(d);
            else
                locked.Add(d);
        }

        StartCoroutine(ShowDecisions(available));
    }

    /// <summary>
    /// Llança l'event corresponent, per actualitzar la UI, per cada decisió
    /// </summary>
    /// <param name="available">Llista de decisons a mostrar</param>
    private IEnumerator ShowDecisions(List<Decision> available)
    {
        yield return new WaitForEndOfFrame();

        foreach (var decision in available)
        {
            OnShowDecision?.Invoke(decision.label, () => OnExchangeDecision(decision));
        }
    }

    /// <summary>
    /// Comprova si una decisió ja s'ha escollit
    /// </summary>
    /// <param name="d">Decisió a comprovar</param>
    /// <returns>True, si s'ha escollit; False, si no s'ha escollit</returns>
    private bool IsAlreadyDone(Decision d)
    {
        if (d.effects == null || d.effects.Length == 0)
            return false;

        // Si la decisió activa un flag i aquest ja és true, ja s'ha donat
        var mainEffect = d.effects.FirstOrDefault();
        if (mainEffect == null) return false;
        return FlagSystem.Instance.GetFlag(mainEffect.flag) == true;
    }

    /// <summary>
    /// Comprova si una decisió compleix els seus requisits
    /// </summary>
    /// <param name="d">Decisió a comprovar</param>
    /// <returns>True, si els requisits es compleixen; False, si no es compleixen</returns>
    private bool MeetsRequirements(Decision d)
    {
        if (d.requires == null || d.requires.Length == 0)
            return true;

        return d.requires.All(req => FlagSystem.Instance.GetFlag(req.flag) == req.value);
    }

    /// <summary>
    /// Comprova si el següent pas d'una decisió de exchange segueix sent part del diàleg o d'un pas nou
    /// </summary>
    /// <param name="decision">Decisió que s'ha pres</param>
    private void OnExchangeDecision(Decision decision)
    {
        DecisionLogic.Execute(decision.id);
        // Decisió interna de conversa -> següent exchange
        if (decision.next.StartsWith("exchange_"))
        {
            ApplyEffects(decision);

            var nextExchange = _currentStep.exchanges.FirstOrDefault(e => e.id == decision.next);
            ShowExchange(nextExchange);
        }
        // Decisió de sortida -> torna al MissionManager
        else
        {
            OnToggleDialogueMenu?.Invoke(false);

            // Desactiva la interacció amb el jugador
            if (FreeRoamHandler.GetRegisteredGameObject(_currentStep.speaker).TryGetComponent(out MissionInteractablePerson person))
            {
                person.EndInteraction();
            }

            MissionManager.Instance.GoToStep(decision.next);
        }
    }

    /// <summary>
    /// Aplica els flags d'una decisió
    /// </summary>
    /// <param name="d">Decisió a aplicar</param>
    private void ApplyEffects(Decision d)
    {
        if (d.effects == null || d.effects.Length == 0)
            return;

        foreach (var effect in d.effects)
            FlagSystem.Instance.SetFlag(effect.flag, effect.value);
    }
    #endregion
}