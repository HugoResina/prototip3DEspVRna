using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Scriptable Objects/Mision/Objective")]
public class MisionObjectiveSO : ScriptableObject
{
    public string Title;

    [TextArea(3, 5)]
    public string Description;
}
