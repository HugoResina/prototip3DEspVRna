using UnityEngine;
using UnityEngine.AI;

public class TurnOffLightsScript : MonoBehaviour
{
    //private NavMeshAgent navMeshAgent;
    //private Animator animator;
    //[SerializeField] Transform destination;
    //bool called = false;
    [SerializeField]
    private GameObject[] Lights;


    
    private void OnEnable()
    {
        WalkieTalkieBehaviourManager.TurnOffLightsEvent += SwitchLights;
    }
    private void OnDisable()
    {
        WalkieTalkieBehaviourManager.TurnOffLightsEvent -= SwitchLights;
    }

    public void SwitchLights()
    {
        if(Lights != null)
        {
            foreach (var light in Lights)
            {
                var lights = light.GetComponentsInChildren<Light>();
                foreach (var item in lights)
                {
                    item.enabled = !item.isActiveAndEnabled;
                }
            }
        }

    }
}
