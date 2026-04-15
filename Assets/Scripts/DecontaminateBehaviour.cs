using UnityEngine;

public class DecontaminateBehaviour : MonoBehaviour
{
    private void OnEnable()
    {
        CarryVictimOutBehaviour.Decontaminate += Decontaminate;
    }
    private void OnDisable()
    {
        CarryVictimOutBehaviour.Decontaminate -= Decontaminate;
    }
    public void Decontaminate()
    {
        Debug.Log("descontaminem a la victima");
    }
}
