using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;


public class CutTrafficPoliceSO : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [SerializeField] Transform destination1;
    [SerializeField] Transform destination2;
    void Start()
    {
        animator = this.GetComponent<Animator>();
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("nav mesh agent component not attached");
        }
        //else
        //{
        //    SetDestination(destination1);
        //}

    }

    private void Update()
    {

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
        {
            animator.SetBool("IsWalking", false);
        }
    }
    private void OnEnable()
    {
        MoveNPC.CutTrafficEvent += CutTraffic;
    }
    private void OnDisable()
    {
       MoveNPC.CutTrafficEvent -= CutTraffic;   
    }

    public void CutTraffic()
    {
      
        if (destination1 != null)
        {
            animator.SetBool("IsWalking", true);
            Vector3 targetVector = destination1.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    
    }
}
