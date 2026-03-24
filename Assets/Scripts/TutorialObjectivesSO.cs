using UnityEngine;

[CreateAssetMenu(fileName = "TutorialObjectives", menuName = "Scriptable Objects/Tutorial/Tutorial Objectives List")]
public class TutorialObjectivesSO : ScriptableObject
{
    [Tooltip("Array ORDENAT d'objectius a realitzar")]
    public TutorialObjectiveSO[] objectives;
}
