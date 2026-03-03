using UnityEngine;
using System;

public class WalkieTalkieBehaviourManager : MonoBehaviour
{
    public static event Action TurnOffLightsEvent;
    public static event Action OpenDoorsEvent;
    public static event Action AlarmEvent;







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

                TurnOffLightsEvent?.Invoke();
                break;
            case 2:

                Debug.Log("asdf");
                break;
            case 3:
                
                AlarmEvent?.Invoke();
                break;

            default:
                break;

        }
    }

}

