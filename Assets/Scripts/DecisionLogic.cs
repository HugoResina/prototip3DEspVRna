using System;
using UnityEditor.ShaderGraph;
using UnityEngine;

public static class DecisionLogic
{
    public static event Action ControlLeakEvent;
    public static event Action ActivateABDEvent;
    public static event Action DisipateGasEvent;
    public static event Action SaveVictimsEvent;
    public static event Action Decontaminate;

    public static void Execute(string decisionId)
    {
        switch (decisionId)
        {
            // Logica de cada decisió específica
            case "d_aparcar_20m":
                FreeRoamHandler.GetRegisteredGameObject("trigger_magatzem_20m").gameObject.SetActive(false);
                break;

            case "d_aparcar_50m":
                FreeRoamHandler.GetRegisteredGameObject("trigger_magatzem_50m").gameObject.SetActive(false);
                break;

            case "d_aparcar_costat_vent":
                FreeRoamHandler.GetRegisteredGameObject("trigger_magatzem_costat_vent").gameObject.SetActive(false);
                break;
             case "d_tornar_muntar_ABD":
                Debug.Log("ABDABDABDABDA");
                ActivateABDEvent?.Invoke();
                break;
            case "d_bomber_entren":
                SaveVictimsEvent?.Invoke();
                break;
            case "d_esperar_frq":
                //?
                break;
            case "d_pai_dianes":
                //?
                break;
            case "d_descontaminacio_quimica": case "d_descontaminacio_encapsulament":
                Decontaminate?.Invoke();
                break;
            case "d_control":
               
                DisipateGasEvent?.Invoke();
                ControlLeakEvent?.Invoke();
                break;
        }
    }
}