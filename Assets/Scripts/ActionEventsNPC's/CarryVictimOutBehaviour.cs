using System;
using UnityEngine;
using UnityEngine.AI;

public class CarryVictimOutBehaviour : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    //public static event Action Decontaminate;
    public Transform destination;
    private bool called = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (called)
        {

            Debug.Log(navMeshAgent.remainingDistance);
            Debug.Log(navMeshAgent.stoppingDistance);

            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                called = false;
                navMeshAgent.isStopped = true;
                navMeshAgent.stoppingDistance = 0.5f;

                animator.SetBool("RestOnFloor", true);
            }
            navMeshAgent.stoppingDistance = 0.5f;
            
        }
    }
    private void OnEnable()
    {
        SaveVictims.GetVictimOut += CarryVictimOut;
    }
    private void OnDisable()
    {
        SaveVictims.GetVictimOut -= CarryVictimOut;
    }
   
    public void RestVictim()
    {

    }
    public void CarryVictimOut()
    {
        if (!called)
        {

            navMeshAgent.SetDestination(destination.position);
            navMeshAgent.stoppingDistance = -1;
           
          
            called = true;


        }

    }


}
