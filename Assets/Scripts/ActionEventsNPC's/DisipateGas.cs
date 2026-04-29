using System.Collections;
using UnityEngine;
using UnityEngine.AI;



public class DisipateGas : MonoBehaviour
{

    public GameObject Truck;
    public GameObject CurtainHose;
    //bool called = false;
    [SerializeField]
    private GameObject Smoke;
    
    private Fade fade;
    SmokeParticlesScript sps;


    private void Start()
    {
        fade = GetComponent<Fade>();
        sps = Smoke.GetComponent<SmokeParticlesScript>();
        
    }

    private void OnEnable()
    {
        DecisionLogic.DisipateGasEvent += DisipateGasCloud;
    }
    private void OnDisable()
    {
        DecisionLogic.DisipateGasEvent -= DisipateGasCloud;
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
        
        yield return new WaitForSeconds(0.5f);
        fade.FadeOut();
    }
}


