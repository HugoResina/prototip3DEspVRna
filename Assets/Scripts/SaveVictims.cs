using System;
using UnityEngine;
using UnityEngine.AI;

public class SaveVictims : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public static event Action GetVictimOut;
    [SerializeField] Transform destination;
    [SerializeField] Transform decontaminationPoint;
    bool called = false;
    [SerializeField]

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
           
                animator.SetBool("IsWalking", false);
                called = false;
                //Smoke.SetActive(false);
                Debug.Log("----------------> At Destination");
                //navMeshAgent.SetDestination(decontaminationPoint.transform.position);
                GetVictimOut?.Invoke();
                this.gameObject.SetActive(false);
            }
        }
    }
    private void OnEnable()
    {
        FireFighterBehaviourManager.SaveVictimsEvent += SaveVictim;
    }
    private void OnDisable()
    {
        FireFighterBehaviourManager.SaveVictimsEvent -= SaveVictim;
    }

    public void SaveVictim()
    {
        called = true;
        if (destination != null)
        {
            animator.SetBool("IsWalking", true);
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);  
        }
    }
  
       
        //podria ser un evento que activase otro objecto con la animacion entera 
    
}


