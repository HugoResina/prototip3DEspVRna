using UnityEngine;

public class MissionObject : MonoBehaviour
{
    [SerializeField] private string _objectName;

    public Decision decision;

    private void Start()
    {
        MissionManager.Instance.RegisterFreeRoamGameObject(_objectName, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        MissionManager.Instance.OnDecisionMade(decision);
    }
}
