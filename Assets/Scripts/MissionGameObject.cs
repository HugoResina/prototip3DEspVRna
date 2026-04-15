using UnityEngine;

public class MissionGameObject : MonoBehaviour
{
    [SerializeField] private string _objectName;

    [HideInInspector] public Decision decision;

    private void Start()
    {
        MissionManager.Instance.RegisterFreeRoamGameObject(_objectName, this);
    }

    public void OnDecisionMade()
    {
        MissionManager.Instance.OnDecisionMade(decision);
    }
}
