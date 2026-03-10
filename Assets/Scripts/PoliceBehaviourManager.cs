using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

public class PoliceBehaviourManager : MonoBehaviour
{
   
    

    public static event Action CutTrafficEvent;
    public static event Action HelpMedicsEvent;




    private void OnEnable()
    {
        GameManager.OnAISend += GetOrder;
    }
    private void OnDisable()
    {
        GameManager.OnAISend -= GetOrder;
    }
    public void GetOrder(int index)
    {
        switch (index)
        {
            case 1:
            
                CutTrafficEvent?.Invoke();
                break;
            case 2:
              
                HelpMedicsEvent?.Invoke();
                break;

            default:
                break;

        }
    }

    
    
}