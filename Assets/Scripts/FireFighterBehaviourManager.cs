using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.ProBuilder.MeshOperations;
public class FireFighterBehaviourManager : MonoBehaviour
{



    public static event Action ControlLeakEvent;
    public static event Action ActivateABDEvent;
    public static event Action DisipateGasEvent;



    

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
            case 6:
                DisipateGasEvent?.Invoke();
                break;
            case 7:
                
                ControlLeakEvent?.Invoke();
                break;
            case 8:
                break;
            case 9:
                
                ActivateABDEvent?.Invoke();
               
                break;
            case 10:
                break;

            default:
                break;

        }
    }


}
