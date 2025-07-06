using UnityEngine;

public class ChangeDialogueAfterFirst : MonoBehaviour
{
    [SerializeField] private DialogueRepeatable _dialogue;
    [SerializeField] private DialogueText _text;
    private bool _inDialogue = false;


    void Update()
    {
        if (_dialogue.InDialogue)
            _inDialogue = true;

        if (_inDialogue && !_dialogue.InDialogue)
        {
            _dialogue.DialogueText = _text;
            enabled = false;
        }
    }
}
