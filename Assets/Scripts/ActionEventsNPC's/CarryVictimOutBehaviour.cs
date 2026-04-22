using System;
using UnityEngine;
using UnityEngine.AI;

public class CarryVictimOutBehaviour : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public static event Action Decontaminate;
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

            //Debug.Log("asdfasdf");
            //Debug.Log(navMeshAgent.remainingDistance);
            //Debug.Log((navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance));     
            Debug.Log(navMeshAgent.remainingDistance);
            Debug.Log(navMeshAgent.stoppingDistance);

            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                called = false;
                navMeshAgent.isStopped = true;
                navMeshAgent.stoppingDistance = 0.5f;

                Debug.Log("llegue");
                //Debug.Log("descontaminem a la victima");
                animator.SetBool("RestOnFloor", true);
                //Decontaminate?.Invoke();
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

            //desctiva/ inmobiliza a los bomberos originals -> referencia GO?
            navMeshAgent.SetDestination(destination.position);
            navMeshAgent.stoppingDistance = -1;
            //activa el objecto de los bomberos cargando a la victima
            //al acabar la animacion desactiva el objecto y mueve a los bomberos originales al punto donde deberian estar
            Debug.Log("sacamos a la victima");
            //bomber1.SetActive(false);
            //bomber2.SetActive(false);
            //bomber3.SetActive(true);
            called = true;


        }

    }


}
