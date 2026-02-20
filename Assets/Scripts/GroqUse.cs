using UnityEngine;

public class GroqUse : MonoBehaviour
{
    void Start()
    {
        GetComponent<GroqChat>().SendMessage("Digues els 13 primers termes de la succeció de Fibonacci", (resp) =>
        {
            Debug.Log($"IA: {resp}");
        });
    }
}
