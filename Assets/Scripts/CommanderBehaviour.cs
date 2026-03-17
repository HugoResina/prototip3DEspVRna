using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.ProBuilder.MeshOperations;
public class CommanderBehaviour : MonoBehaviour
{
   

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
                Debug.Log("no fa vent");
                break;
            case 2:
                Debug.Log("fuga de quimics");
                break;
            case 3:
                Debug.Log("amoniac");
                break;
            default:
                break;

        }
    }


}

