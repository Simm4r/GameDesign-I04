using System;
using UnityEngine;

public class TutorialAfterDialogueTrigger : MonoBehaviour
{
    private TriggerProximityHandler _handler;
    private EventDialogue _dialogue;
    [SerializeField] private TutorialHandler.TutorialPage _tutorialPage;
    private enum TriggerType
    {
        Proximity,
        Dialogue
    }
    [SerializeField] private TriggerType _triggerType = TriggerType.Proximity;
    void Awake()
    {
        _handler = GetComponent<TriggerProximityHandler>();
        _dialogue = GetComponent<EventDialogue>();
    }

    void Update()
    {
        if ((_triggerType == TriggerType.Proximity && !_handler.DialogueFinished) || (_triggerType == TriggerType.Dialogue && !_dialogue.DialogueFinished))
            return;

        if (TutorialHandler.Instance.GetActualState() == "Idle")
        {
            Player.Instance.Tutorial = true;
            HandleTutorialPage();
        }
        else if (TutorialHandler.Instance.GetActualState() == "OnScreen")
        {
            if(PlayerInput.Instance.DialogueNext) 
                HandleTutorial();
        }
    }

    private void HandleTutorial()
    {
        switch (_tutorialPage)
        {
            case TutorialHandler.TutorialPage.Pos1:
                TutorialHandler.Instance.SetActualTutorial(TutorialHandler.TutorialPage.Pos2);
                _tutorialPage = TutorialHandler.TutorialPage.Pos2;
                break;
            case TutorialHandler.TutorialPage.Pos2:
                TutorialHandler.Instance.Hide();
                enabled = false;
                break;
            case TutorialHandler.TutorialPage.SV:
                TutorialHandler.Instance.Hide();
                enabled = false;
                break;
            case TutorialHandler.TutorialPage.PosEn2:
                TutorialHandler.Instance.Hide();
                enabled = false;
                break; 
        }
    }

    private void HandleTutorialPage()
    {
        switch (_tutorialPage)
        {
            case TutorialHandler.TutorialPage.Pos1:
                TutorialHandler.Instance.SetActualTutorial(TutorialHandler.TutorialPage.Pos1);
                break;
            case TutorialHandler.TutorialPage.SV:
                TutorialHandler.Instance.SetActualTutorial(TutorialHandler.TutorialPage.SV);
                break;
            case TutorialHandler.TutorialPage.PosEn2:
                TutorialHandler.Instance.SetActualTutorial(TutorialHandler.TutorialPage.PosEn2);
                break;
        }
        TutorialHandler.Instance.Show();
    }
}
