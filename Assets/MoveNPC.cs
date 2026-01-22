using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.ProBuilder.MeshOperations;

public class MoveNPC : MonoBehaviour
{
    [SerializeField] Transform destination1;
    [SerializeField] Transform destination2;
    private Animator animator;
    

    NavMeshAgent navMeshAgent;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        STT.OnSend += GetOrder;
    }
    private void OnDisable()
    {
        STT.OnSend -= GetOrder;
    }
    public void GetOrder(int index)
    {
        switch (index)
        {
            case 1:
                SetDestination(destination1);
                break;
            case 2:
                SetDestination(destination2);
                break;
            default:
                break;

        }
    }

    private void SetDestination(Transform destination)
    {
        if (destination != null)
        {
            animator.SetBool("IsWalking", true);
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }
}