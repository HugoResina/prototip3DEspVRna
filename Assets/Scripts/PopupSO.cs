using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Popup", menuName = "Scriptable Objects/Pop-up")]
public class PopupSO : ScriptableObject
{
    public string title;
    public Image image;

    [TextArea(3, 5)]
    public string content;
}
