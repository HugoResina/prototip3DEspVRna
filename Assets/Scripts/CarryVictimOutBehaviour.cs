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
    //public GameObject bomber1;
    //public GameObject bomber2;
    //public GameObject bomber3;

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
            Debug.Log(navMeshAgent.remainingDistance);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                called = false;
                navMeshAgent.isStopped = true;
                Debug.Log("llegue");
                //Debug.Log("descontaminem a la victima");
                Decontaminate?.Invoke();
            }
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
   
    public void CarryVictimOut()
    {
        if (!called)
        {
            called = true;

            //desctiva/ inmobiliza a los bomberos originals -> referencia GO?

            //activa el objecto de los bomberos cargando a la victima
            //al acabar la animacion desactiva el objecto y mueve a los bomberos originales al punto donde deberian estar
            Debug.Log("sacamos a la victima");
            //bomber1.SetActive(false);
            //bomber2.SetActive(false);
            //bomber3.SetActive(true);
            navMeshAgent.SetDestination(destination.position);
        }

    }


}
