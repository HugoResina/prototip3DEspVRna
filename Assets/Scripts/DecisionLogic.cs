using System;
using UnityEditor.ShaderGraph;
using UnityEngine;

public static class DecisionLogic
{
    #region Logic Events
    // Events que es llancen per executar la lògica de cada decisió
    public static event Action ControlLeakEvent;
    public static event Action ActivateABDEvent;
    public static event Action DisipateGasEvent;
    public static event Action SaveVictimsEvent;
    public static event Action Decontaminate;
    #endregion

    #region Decision Logics
    /// <summary>
    /// Executa la lògica de les decisions
    /// </summary>
    /// <param name="decisionId">Id de la decisió de la qual es vol executar la lògica</param>
    public static void Execute(string decisionId)
    {
        switch (decisionId)
        {
            // Logica de cada decisió específica
             case "d_tornar_muntar_ABD":
                
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
    #endregion
}