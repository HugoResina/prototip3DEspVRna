using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    public static event Action<bool> OnToggleDialogueMenu;
    public static event Action<string> OnUpdateDialogueText;
    public static event Action<string, Action> OnShowDecision;

    private MissionStep _currentStep;

    private void Awake()
    {
        OnToggleDialogueMenu?.Invoke(false);
    }

    public void StartDialogue(MissionStep step)
    {
        OnToggleDialogueMenu?.Invoke(true);

        _currentStep = step;

        // Mostra el primer exchange
        ShowExchange(step.exchanges[0]);
    }

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

    private IEnumerator ShowDecisions(List<Decision> available)
    {
        yield return new WaitForEndOfFrame();

        foreach (var decision in available)
        {
            Debug.Log($"DIALOGUE HANDLER: Available decision -> {decision.label}");
            OnShowDecision?.Invoke(decision.label, () => OnExchangeDecision(decision));
        }
    }

    private bool IsAlreadyDone(Decision d)
    {
        if (d.effects == null || d.effects.Length == 0)
            return false;

        // Si la decisió activa un flag i aquest ja és true, ja s'ha donat
        var mainEffect = d.effects.FirstOrDefault();
        if (mainEffect == null) return false;
        return FlagSystem.Instance.GetFlag(mainEffect.flag) == true;
    }

    private bool MeetsRequirements(Decision d)
    {
        if (d.requires == null || d.requires.Length == 0)
            return true;

        return d.requires.All(req => FlagSystem.Instance.GetFlag(req.flag) == req.value);
    }

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

            if (FreeRoamHandler.GetRegisteredGameObject(_currentStep.speaker).TryGetComponent(out MissionInteractablePerson person))
            {
                person.EndInteraction();
            }

            MissionManager.Instance.GoToStep(decision.next);
        }
    }

    private void ApplyEffects(Decision d)
    {
        if (d.effects == null || d.effects.Length == 0)
            return;

        foreach (var effect in d.effects)
            FlagSystem.Instance.SetFlag(effect.flag, effect.value);
    }
}