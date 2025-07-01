using UnityEngine;

public class TriggerProximityHandler : MonoBehaviour
{
    [SerializeField] private float _triggerDistance = 1f;
    [SerializeField] private Talkable _dialogue;
    private bool _dialogueFinished = false;

    public bool DialogueFinished
    {
        get { return _dialogueFinished; }
        set { _dialogueFinished = value; }
    }

    void Update()
    {
        if (_dialogueFinished)
            return;

        float distance = Vector3.Distance(Player.Instance.transform.position, transform.position);

        if (distance > _triggerDistance || ScreenFadeController.Instance.IsFading)
            return;

        if (!Player.Instance.InDialogue)
            Player.Instance.InDialogue = true;

        _dialogue.Talk();
    }
}
