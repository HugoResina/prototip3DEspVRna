using UnityEngine;

public class ExitCInematic : MonoBehaviour
{
    public GameObject Player;
    public GameObject cameraCinematic;

    public void ExitCinematic()
    {
        ObjectiveList.Instance.ShowNext();
        //UIManager.Instance.
        Debug.Log("sortir de la cinematica");
        //call on last frame of cinematic 
        Player.SetActive(true);
        cameraCinematic.SetActive(false);
    }
}
