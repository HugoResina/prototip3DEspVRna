using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    public void StartDialogue(MissionStep step)
    {
        // Mostra el primer exchange
        ShowExchange(step.exchanges[0]);
    }

    private void ShowExchange(Exchange exchange)
    {
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
    }

    private bool IsAlreadyDone(Decision d)
    {
        // Si la decisió activa un flag i aquest ja és true, ja s'ha donat
        var mainEffect = d.effects.FirstOrDefault();
        if (mainEffect == null) return false;
        return FlagSystem.Instance.GetFlag(mainEffect.flag) == true;
    }

    private bool MeetsRequirements(Decision d)
    {
        return d.requires.All(req =>
            FlagSystem.Instance.GetFlag(req.flag) == (req.value == "true"));
    }

    private void OnExchangeDecision(MissionStep currentStep, Decision decision)
    {
        // Decisió interna de conversa -> següent exchange
        if (decision.next.StartsWith("exchange_"))
        {
            var nextExchange = currentStep.exchanges.FirstOrDefault(e => e.id == decision.next);
            ShowExchange(nextExchange);
        }
        // Decisió de sortida -> torna al MissionManager
        else
        {
            MissionManager.Instance.GoToStep(decision.next);
        }
    }
}