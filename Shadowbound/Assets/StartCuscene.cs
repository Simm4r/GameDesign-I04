using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartCuscene : MonoBehaviour
{
    public static StartCuscene Instance { get; private set; }
    private DialogueRepeatable _dialogue;
    public event Action OnCutsceneEnd;
    private bool _cutsceneStarted = false;
    private bool _startedFinalCorutine = false;
    private bool _dialogueFinished = false;
    private Canvas _canvas;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _canvas = GetComponent<Canvas>();
    }
    public void StartCutscene(Image _image, DialogueRepeatable dialogue)
    {
        _dialogue = dialogue;
        _cutsceneStarted = false;
        _startedFinalCorutine = false;
        _dialogueFinished = false;
        if (_image != null)
        {
            var image = GetComponent<Image>();
            image.sprite = _image.sprite;
        }
        StartCoroutine(StartCutsceneC());
    }

    IEnumerator StartCutsceneC()
    {
        Player.Instance.InCutscene = true;
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSecondsRealtime(1.5f);
        _canvas.enabled = true;
        yield return new WaitForSecondsRealtime(0.5f);
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSecondsRealtime(1.0f);
        _dialogue.Talk();
        _cutsceneStarted = true;
    }

    void Update()
    {
        if (Player.Instance.InCutscene && _cutsceneStarted)
        {
            if (!_dialogue.InDialogue)
                _dialogueFinished = true;
                
            if (_dialogueFinished && !_startedFinalCorutine)
            {
                _startedFinalCorutine = true;
                StartCoroutine(EndCutscene());
                return;
            }
            else if(!_dialogueFinished)
                _dialogue.Talk();
        }
    }

    IEnumerator EndCutscene()
    {
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSecondsRealtime(1.0f);
        _canvas.enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSecondsRealtime(1.5f);
        DialogueController.Instance.gameObject.SetActive(false);
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSecondsRealtime(1.0f);
        Player.Instance.InCutscene = false;
        OnCutsceneEnd?.Invoke();
        Debug.Log("Invoco il metodo");
    }
}
