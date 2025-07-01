using System;
using UnityEngine;

public class EventDialogue : Talkable
{
    [SerializeField] private EventTriggerer _trigger;
    [SerializeField] private DialogueText _dialogueText;
    private bool firstLine = true;
    private bool _triggered = false;
    private bool _dialogueFinished = false;

    void OnEnable()
    {
        _trigger.OnReadFinished += triggerDialogue;
    }

    void OnDisable()
    {
        _trigger.OnReadFinished -= triggerDialogue;
    }

    private void triggerDialogue()
    {
        Player.Instance.InDialogue = true;
        _triggered = true;
        Talk();
    }

    public override void Talk()
    {
        if (firstLine)
        {
            DialogueController.Instance.DisplayNextParagraph(_dialogueText);
            firstLine = false;
        }
        else if (PlayerInput.Instance.DialogueNext)
            DialogueController.Instance.DisplayNextParagraph(_dialogueText);

        if (DialogueController.Instance.CanExit && !_dialogueFinished)
        {
            _dialogueFinished = true;
            DialogueController.Instance.CanExit = false;
        }
    }

    void Update()
    {
        if (_dialogueFinished || !_triggered)
            return;

        Talk();
    }
}
