using UnityEngine;

public class DialogueRepeatable : Talkable
{
    [SerializeField] private DialogueText _dialogueText;
    public DialogueText DialogueText
    {
        set => _dialogueText = value;
    }
    private bool _firstLine = true;

    private bool _inDialogue = false;
    public bool InDialogue => _inDialogue;
    public override void Talk()
    {
        if (_firstLine)
        {
            _inDialogue = true;
            DialogueController.Instance.DisplayNextParagraph(_dialogueText);
            _firstLine = false;
        }
        else if (PlayerInput.Instance.DialogueNext)
        {
            DialogueController.Instance.DisplayNextParagraph(_dialogueText);
        }
        if (DialogueController.Instance.CanExit)
        {
            _inDialogue = false;
            DialogueController.Instance.CanExit = false;
            _firstLine = true;
        }
    }

    public void SetDialogue(DialogueText _newDialogue)
    {
        _dialogueText = _newDialogue;
    }
}
