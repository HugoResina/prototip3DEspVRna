using UnityEngine;

public class MissionTrigger : MissionGameObject
{
    // Trigger Collider amb el que col·lisionar
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        if (_collider == null)
        {
            Debug.LogError("MISSION TRIGGER: Missing a Collider component.");
        }
        else
        {
            _collider.isTrigger = true; // Inicialitzar com de tipus trigger
        }
    }

    /// <summary>
    /// En entrar al collider es pren la decisió
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerInteraction _))
        {
            OnDecisionMade();
        }
    }
}
