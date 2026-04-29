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
        DecisionLogic.Decontaminate += Decontaminate;
    }
    private void OnDisable()
    {
        DecisionLogic.Decontaminate -= Decontaminate;
    }
    public void Decontaminate()
    {
      
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
