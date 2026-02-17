using UnityEngine;

[CreateAssetMenu(fileName = "PromptBaseSO", menuName = "Scriptable Objects/PromptBaseSO")]
public class PromptBaseSO : ScriptableObject
{
    [TextArea(10, 50)]
    public string Prompt;
}
