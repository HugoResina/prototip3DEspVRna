using UnityEngine;

public class MissionGameObject : MonoBehaviour
{
    // Identificador del MissionGameObject
    [SerializeField] private string _objectName;

    // Decisió a la qual pertany
    [HideInInspector] public Decision decision;

    private void Start()
    {
        // Es registra en començar
        MissionManager.Instance.RegisterFreeRoamGameObject(_objectName, this);
    }

    /// <summary>
    /// Avisa a MissionManager que la decisió s'ha dut a terme
    /// </summary>
    public void OnDecisionMade()
    {
        MissionManager.Instance.OnDecisionMade(decision);
    }
}
