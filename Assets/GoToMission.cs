using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GoToMission : MonoBehaviour
{
    [SerializeField]
    private List<MisionSO> misions;
    [SerializeField]
    private GameObject vehicle;
    private VehicleToMission VTM;

    private void OnEnable()
    {
        VTM = vehicle.GetComponent<VehicleToMission>();
        WalkieTalkieBehaviourManager.GoToMission += goToMission;
    }
    private void OnDisable()
    {
        WalkieTalkieBehaviourManager.GoToMission -= goToMission;
    }

    public void goToMission(int num)
    {
        
        if (vehicle != null && vehicle.activeInHierarchy == false)
        {
            vehicle.SetActive(true);
        }
        VTM.mission = misions.Find(m => m.Id.Equals(num));

    }
}
