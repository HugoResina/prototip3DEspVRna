using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Scriptable Objects/Mision/Tutorial Objective")]
public class TutorialObjectiveSO : MisionObjectiveSO
{
    public TutorialObjectiveType tutorialType;
}

public enum TutorialObjectiveType
{
    Look, Move, Sprint, Attack, Interact, Exit
}