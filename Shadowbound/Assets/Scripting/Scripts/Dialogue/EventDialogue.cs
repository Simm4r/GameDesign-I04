using System;
using System.Collections;
using UnityEngine;

public class EventDialogue : Talkable
{
    [SerializeField] private EventTriggerer _triggerE;
    [SerializeField] private DialogueText _dialogueText;
    private bool firstLine = true;
    private bool _triggered = false;
    private bool _dialogueFinished = false;
    public bool DialogueFinished
    {
        get => _dialogueFinished;
    }

    void OnEnable()
    {
        if (_triggerE)
            _triggerE.OnReadFinished += TriggerDialogue;
        else
        {
            StartCoroutine(WaitForSub());
        }
    }
 
    IEnumerator WaitForSub()
    {
        yield return new WaitUntil(() => StartCuscene.Instance != null);
        Debug.Log("Subscribed to cutscene end");
        StartCuscene.Instance.OnCutsceneEnd += TriggerDialogue;
    }

    void OnDisable()
    {
        if (_triggerE)
            _triggerE.OnReadFinished -= TriggerDialogue;
        else
        {
            Debug.Log("Unsubscribed to cutscene end");
            StartCuscene.Instance.OnCutsceneEnd -= TriggerDialogue;
        }
    }

    private void TriggerDialogue()
    {
        Debug.Log("DialogueStart");
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
        {
            if (!DialogueController.Instance.IsTyping)
                HUDAudioHandler.Instance.StartSound();
            DialogueController.Instance.DisplayNextParagraph(_dialogueText);
        }
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
