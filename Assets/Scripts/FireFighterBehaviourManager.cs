using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.ProBuilder.MeshOperations;
public class FireFighterBehaviourManager : MonoBehaviour
{



    //public static event Action TurnOffFire;
    public static event Action ControlLeakEvent;
    public static event Action ActivateABDEvent;




    

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
            //case 6:

            //    TurnOffFire?.Invoke();
            //    break;
            case 7:
                
                ControlLeakEvent?.Invoke();
                break;
            case 9:
                
                   ActivateABDEvent?.Invoke();
               
                    break;

            default:
                break;

        }
    }


}
