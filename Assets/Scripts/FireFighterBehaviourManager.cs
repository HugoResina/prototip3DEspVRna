using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.ProBuilder.MeshOperations;
public class FireFighterBehaviourManager : MonoBehaviour
{



    public static event Action TurnOfFire;
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
            case 1:

                TurnOfFire?.Invoke();
                break;
            case 2:

                ActivateABDEvent?.Invoke();
                break;

            default:
                break;

        }
    }


}
