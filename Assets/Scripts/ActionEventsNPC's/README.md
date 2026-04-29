# Afegir comportament de NPc's i/o events a punts del arxiu JSON de una missió

<b style='color:red;'>Primer es recomana estar familiatrizat amb els continguts del arxiu JSON de la missió, informació pertintent: [JSON Missió](https://github.com/HugoResina/prototip3DEspVRna/tree/main/Assets/Resources#readme)</b>

- Suposem un script que defineix una acció que ha de succeir en un punt de la missió:
aquest script en concret fa aparèixer un nou camió i l'acortinador i crida a un mètode d'una classe SmokeParticlesScript que limita la emissió del objecte Smoke perquè sembli que la fuita està sent controlada pel flux d'aigua del acortinador

```c#
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DisipateGas : MonoBehaviour
{

    public GameObject Truck;
    public GameObject CurtainHose;
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
        Debug.Log("activat");
        yield return new WaitForSeconds(0.5f);
        fade.FadeOut();
    }
}
```

- Volem que tot això passi en un punt concret de la interacció, com seria donar l'ordre de dissipar el gas als bombers en aquest element d'un bloc decisions.

```json
 {
    "decisionId": "d_control",
    "label": "Ordeno mantenir el control del flux i reforçar amb personal la maniobra de atacar la font",
    "effects": [
        { 
            "flag": "ordre_control", "value": true 
        }
    ],
    "next": "exchange_control"
}
```

- Per tal que en aquest punt s'executi la lògica d'abans, afegirem un block case + "decisionId" al Switch case de la classe DecisionLogic:

```c#
using System;
using UnityEditor.ShaderGraph;
using UnityEngine;

public static class DecisionLogic
{
    //[...]
    public static event Action DisipateGasEvent;

    public static void Execute(string decisionId)
    {
        switch (decisionId)
        {
            case "d_control":
    
            DisipateGasEvent?.Invoke();
            
            break;
            //[...]
```
