using System.Collections;
using UnityEngine;

public class StartCall : MonoBehaviour
{
    private void Start()
    {

        StartCallLate();


    }
    private IEnumerable StartCallLate()
    {
        yield return new WaitForSeconds(0.2f);
        InteractablePerson IPer = GetComponent<InteractablePerson>();
        IPer.Interact(gameObject);
    }
}
