using System;
using UnityEngine;

public class ExitCInematic : MonoBehaviour
{
    public GameObject Player;
    public GameObject cameraCinematic;
    public static event Action StartMission;

    public void ExitCinematic()
    {
        ObjectiveList.Instance.ShowNext();
        Debug.Log("sortir de la cinematica");
        Player.SetActive(true);
        cameraCinematic.SetActive(false);
        StartMission?.Invoke();
    }
}
