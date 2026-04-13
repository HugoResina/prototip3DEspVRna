using UnityEngine;

public static class DecisionLogic
{
    public static void Execute(string decisionId)
    {
        switch (decisionId)
        {
            // Logica de cada decisió específica
            case "d_aparcar_20m":
                FreeRoamHandler.GetRegisteredGameObject("trigger_magatzem_20m").SetActive(false);
                break;

            case "d_aparcar_50m":
                FreeRoamHandler.GetRegisteredGameObject("trigger_magatzem_50m").SetActive(false);
                break;

            case "d_aparcar_costat_vent":
                FreeRoamHandler.GetRegisteredGameObject("trigger_magatzem_costat_vent").SetActive(false);
                break;
        }
    }
}