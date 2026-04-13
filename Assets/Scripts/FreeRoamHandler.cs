using System.Collections.Generic;
using UnityEngine;

public class FreeRoamHandler : MonoBehaviour
{
    private static Dictionary<string, GameObject> _registry = new Dictionary<string, GameObject>();

    public void RegisterGameObject(string id, GameObject obj)
    {
        _registry[id] = obj;
        obj.SetActive(false);
    }

    public static GameObject GetRegisteredGameObject(string id)
    {
        return _registry.TryGetValue(id, out var obj) ? obj : null;
    }

    public void ActivateForStep(MissionStep step)
    {
        // Desactiva tots primer
        foreach (var kv in _registry)
            kv.Value.SetActive(false);

        // Activa només els d'aquest step
        foreach (var id in step.activate)
        {
            if (_registry.TryGetValue(id, out var obj))
            {
                obj.SetActive(true);

                foreach (var decision in step.decisions)
                {
                    if (decision.triggeredBy == id)
                    {
                        obj.GetComponent<MissionObject>().decision = decision;
                    }
                }
            }
        }
    }
}