using System.Collections.Generic;
using UnityEngine;

public class FreeRoamHandler : MonoBehaviour
{
    private Dictionary<string, GameObject> _registry = new Dictionary<string, GameObject>();

    public void RegisterGameObject(string id, GameObject obj)
        => _registry[id] = obj;

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
                Debug.Log($"Activating {id} for step {step.id}");
            }
        }
    }
}