using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.ProBuilder.MeshOperations;

public class MoveNPC : MonoBehaviour
{
   
    

    public static event Action CutTrafficEvent;
    public static event Action HelpMedicsEvent;
    

    

  

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
                //SetDestination(destination1);
                Debug.Log("entro a 1");
                CutTrafficEvent?.Invoke();
                break;
            case 2:
                //SetDestination(destination2);
                HelpMedicsEvent?.Invoke();
                break;
            default:
                break;

        }
    }

    
    
}