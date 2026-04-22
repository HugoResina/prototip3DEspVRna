using UnityEngine;
using UnityEngine.AI;

public class DecontaminateBehaviour : MonoBehaviour
{
    private Animator animator;
    public GameObject blanket;
    private NavMeshAgent navMeshAgent;
    public Transform destination;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        FireFighterBehaviourManager.Decontaminate += Decontaminate;
    }
    private void OnDisable()
    {
        FireFighterBehaviourManager.Decontaminate -= Decontaminate;
    }
    public void Decontaminate()
    {
        //anim 

        Debug.Log("descontaminem a la victima");
        animator.SetBool("RestOnFloor", false);
        animator.SetBool("Treated", true);
    }
    public void WalkAgain()
    {

        animator.SetBool("Walking", true);
        animator.SetBool("RestOnFloor", false);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination.position);
    }
    public void ThermicBlancket()
    {
        blanket.SetActive(true);
    }
}
