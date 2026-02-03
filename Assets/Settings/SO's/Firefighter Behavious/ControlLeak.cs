using UnityEngine;
using UnityEngine.AI;

public class ControlLeak : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [SerializeField] Transform destination;
    bool called = false;


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
        if (called)
        {


            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
            {
                //animacio acortinador?
                animator.SetBool("IsWalking", false);
                called = false;
            }
        }
    }
    private void OnEnable()
    {
        FireFighterBehaviourManager.ControlLeakEvent += ControlLeaK;
    }
    private void OnDisable()
    {
        FireFighterBehaviourManager.ControlLeakEvent -= ControlLeaK;
    }

    public void ControlLeaK()
    {
        called = true;
        if (destination != null)
        {

            animator.SetBool("IsWalking", true);
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }

    }
}
