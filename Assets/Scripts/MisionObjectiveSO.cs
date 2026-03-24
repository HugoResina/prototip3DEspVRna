using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Scriptable Objects/MisionObjective")]
public class MisionObjectiveSO : ScriptableObject
{
    public string title;

    [TextArea(3, 5)]
    public string description;
}
