using UnityEngine;

public class SingleDialogue : Talkable
{
    [SerializeField] private DialogueText _dialogueText;
    [SerializeField] private TriggerProximityHandler _handler;
    private bool firstLine = true;
    public override void Talk()
    {
        if (firstLine)
        {
            DialogueController.Instance.DisplayNextParagraph(_dialogueText);
            firstLine = false;
        }
        else if (PlayerInput.Instance.DialogueNext)
        {
            DialogueController.Instance.DisplayNextParagraph(_dialogueText);

        }

        if (DialogueController.Instance.CanExit && !_handler.DialogueFinished)
        {
            _handler.DialogueFinished = true;
            DialogueController.Instance.CanExit = false;
        }
            
            
    }

    
}
