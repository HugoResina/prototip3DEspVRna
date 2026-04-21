using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DisipateGas : MonoBehaviour
{

    //private NavMeshAgent navMeshAgent;
    //private Animator animator;
    public GameObject Truck;
    public GameObject CurtainHose;
    //[SerializeField] 
    bool called = false;
    [SerializeField]
    private GameObject Smoke;
    
    private Fade fade;
    SmokeParticlesScript sps;


    private void Start()
    {
        fade = GetComponent<Fade>();
        sps = Smoke.GetComponent<SmokeParticlesScript>();
        
    }

    private void Update()
    {
        //if (called)
        //{


        //    //if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
        //    //{
        //    //    //animacio acortinador?
        //    //    animator.SetBool("IsWalking", false);
        //    //    called = false;
        //        //Smoke.SetActive(false);
        //        SmokeParticlesScript sps = Smoke.GetComponent<SmokeParticlesScript>();
        //        //sps.StartToDisipate();
        //    //}
        //}
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
        StartCoroutine(DisipateRoutine());
    }
    public IEnumerator DisipateRoutine()
    {
        fade.FadeIn();
        yield return new WaitForSeconds(1f);
        Truck.SetActive(true);
        CurtainHose.SetActive(true);
        sps.CurtainEffect();
        Debug.Log("activat");
        yield return new WaitForSeconds(0.5f);
        fade.FadeOut();
    }
}


