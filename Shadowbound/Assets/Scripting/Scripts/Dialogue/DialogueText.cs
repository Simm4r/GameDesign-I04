using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Container")]
public class DialogueText : ScriptableObject
{
   public DialogueParagraph[] paragraphs;
}

[System.Serializable]
public class DialogueParagraph
{
    public string speakerName;
    [TextArea(5, 10)]
    public string text;
}