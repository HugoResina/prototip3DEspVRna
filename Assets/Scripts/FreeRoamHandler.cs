using System.Collections.Generic;
using UnityEngine;

public class FreeRoamHandler : MonoBehaviour
{
    // Diccionari de GameObjects registrats
    private static Dictionary<string, MissionGameObject> _registry = new Dictionary<string, MissionGameObject>();

    #region FreeRoam Registry
    /// <summary>
    /// Registra un GameObject
    /// </summary>
    /// <param name="id">Indentificador del nou gameobject registrat</param>
    /// <param name="obj">Objecte a registrar</param>
    public void RegisterGameObject(string id, MissionGameObject obj)
    {
        _registry[id] = obj;
        
        if (obj.TryGetComponent(out MissionTrigger _) || obj.TryGetComponent(out MissionWalkie _))
        {
            obj.gameObject.SetActive(false);
        }
        else if (obj.TryGetComponent(out MissionInteractablePerson person))
        {
            person.enabled = false;
        }
    }

    /// <summary>
    /// Busca un MissionGameObject pel seu id i el retorna
    /// </summary>
    /// <param name="id">Identifcador del MissionGameObject a buscar</param>
    /// <returns>MissionGameObject trobat</returns>
    public static MissionGameObject GetRegisteredGameObject(string id)
    {
        return _registry.TryGetValue(id, out var obj) ? obj : null;
    }
    #endregion

    #region GameObject Activation
    /// <summary>
    /// Activa o desactiva els MissionGameObject segons el pas i el tipus de MissionGameObject
    /// </summary>
    /// <param name="step">Pas</param>
    public void ActivateForStep(MissionStep step)
    {
        // Desactiva tots primer
        foreach (var kv in _registry)
        {
            if (kv.Value.TryGetComponent(out MissionTrigger trigger) || kv.Value.TryGetComponent(out MissionWalkie _))
            {
                kv.Value.gameObject.SetActive(false);
            }
            else if (kv.Value.TryGetComponent(out MissionInteractablePerson person))
            {
                person.enabled = false;
            }
        }

        // Activa només els d'aquest step
        foreach (var id in step.activate)
        {
            if (_registry.TryGetValue(id, out var obj))
            {
                if (obj.TryGetComponent(out MissionTrigger _)|| obj.TryGetComponent(out MissionWalkie _))
                {
                    obj.gameObject.SetActive(true);
                }
                else if (obj.TryGetComponent(out MissionInteractablePerson person))
                {
                    person.enabled = true;
                }

                foreach (var decision in step.decisions)
                {
                    if (decision.triggeredBy == id)
                    {
                        obj.GetComponent<MissionGameObject>().decision = decision;
                    }
                }
            }
        }
    }
    #endregion
}