using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Scriptable Objects/Tutorial/Tutorial Objective")]
public class TutorialObjectiveSO : MisionObjectiveSO
{
    public TutorialObjectiveType tutorialType;
    public PopupSO popup;
}

public enum TutorialObjectiveType
{
    Look, Move, Sprint, Pickup, Rotate, Relese
}