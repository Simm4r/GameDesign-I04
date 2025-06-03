using UnityEngine;

public class SingleDialogue : Talkable
{
    [SerializeField] private DialogueController _dialogueController;
    [SerializeField] private DialogueText _dialogueText;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private TriggerProximityHandler _handler;
    private bool firstLine = true;
    public override void Talk()
    {
        if (firstLine)
        {
            _dialogueController.DisplayNextParagraph(_dialogueText);
            firstLine = false;
        }
        else if (_input.DialogueNext)
        {
            _dialogueController.DisplayNextParagraph(_dialogueText);

        }

        if (_dialogueController.CanExit && !_handler.DialogueFinished)
            _handler.DialogueFinished = true;
    }

    
}
