using UnityEngine;

public class MissionTrigger : MissionGameObject
{
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
            _collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnDecisionMade();
    }
}
