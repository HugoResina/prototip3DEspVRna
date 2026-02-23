using UnityEngine;
using UnityEngine.AI;

public class DisipateGas : MonoBehaviour
{
   
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [SerializeField] Transform destination;
    bool called = false;
    [SerializeField]
    private GameObject Smoke;


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
                //Smoke.SetActive(false);
                SmokeParticlesScript sps = Smoke.GetComponent<SmokeParticlesScript>();
                sps.StartToDisipate();
            }
        }
    }
    private void OnEnable()
    {
        FireFighterBehaviourManager.DisipateGasEvent += DisipateGasCloud;
    }
    private void OnDisable()
    {
        FireFighterBehaviourManager.DisipateGasEvent -= DisipateGasCloud;
    }

    public void DisipateGasCloud()
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


