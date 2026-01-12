using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MoveNPC : MonoBehaviour
{
    [SerializeField] Transform destination1;
    [SerializeField] Transform destination2;
    

    NavMeshAgent navMeshAgent;

    void Start()
    {
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
        if (Input.GetKeyDown("a"))
        {
            Debug.Log("A");

            SetDestination(destination1);
        }
        else if (Input.GetKeyDown("s"))
        {
            Debug.Log("S");

            SetDestination(destination2);
        }
    }

    private void SetDestination(Transform destination)
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }
}