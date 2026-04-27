using System.Collections.Generic;
using UnityEngine;

public class FreeRoamHandler : MonoBehaviour
{
    private static Dictionary<string, MissionGameObject> _registry = new Dictionary<string, MissionGameObject>();

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

    public static MissionGameObject GetRegisteredGameObject(string id)
    {
        return _registry.TryGetValue(id, out var obj) ? obj : null;
    }

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
                    Debug.Log("FREE ROAM HANDLER: Activated -> " + id);
                }
                else if (obj.TryGetComponent(out MissionInteractablePerson person))
                {
                    person.enabled = true;
                    Debug.Log($"FREE ROAM HANDLER: Activated MissionInteractablePerson -> {id} ({person.gameObject.name})");
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
}