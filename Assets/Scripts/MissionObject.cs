using UnityEngine;

public class MissionObject : MonoBehaviour
{
    [SerializeField] private string _objectName;

    private void Start()
    {
        MissionManager.Instance.RegisterFreeRoamGameObject(_objectName, gameObject);
    }
}
