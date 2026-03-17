using UnityEngine;

public class StartCall : MonoBehaviour
{
    private void Start()
    {
        InteractablePerson IPer = GetComponent<InteractablePerson>();
        IPer.Interact(gameObject);
    }
}
