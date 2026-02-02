using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class SetABDSafeZone : MonoBehaviour
{
   
   

    private Fade fade;
    public GameObject ABD;


    private void Start()
    {
        fade = GetComponent<Fade>();
    }
  
    private void OnEnable()
    {
        MoveNPC.ActivateABDEvent += ActivateABDZone;
    }
    private void OnDisable()
    {
        MoveNPC.ActivateABDEvent -= ActivateABDZone;
    }

    public void ActivateABDZone()
    {
        StartCoroutine(ActivateABDZoneRoutine());
    }
    private IEnumerator ActivateABDZoneRoutine()
    {
        fade.FadeIn();
        yield return new WaitForSeconds(1f);
        ABD.SetActive(true);
        Debug.Log("activat");
        yield return new WaitForSeconds(0.5f);
        fade.FadeOut();

        
    }
}
