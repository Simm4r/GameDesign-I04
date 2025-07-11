using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI dialogueTextMesh;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private float typeSpeed = 10;

    private Queue<DialogueParagraph> paragraphs = new Queue<DialogueParagraph>();
    private bool conversationEnded;
    private bool isTyping;
    public bool IsTyping
    {
        get => isTyping;
    }
    private string p;
    private Coroutine typeDialogueCoroutine;
    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;
    private bool _canExit = false;

    public bool CanExit
    {
        get { return _canExit; }
        set { _canExit = value; }
    }
    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                StartConversation(dialogueText);
            }
            else if (conversationEnded && !isTyping)
            {
                _canExit = true;
                EndConversation();
                Player.Instance.InDialogue = false;

                return;
            }
        }

        if (!isTyping)
        {
            DialogueParagraph paragraph = paragraphs.Dequeue();
            p = paragraph.text;
            speakerNameText.text = paragraph.speakerName;

            typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
        }
        else
        {
            FinishParagraphEarly();
        }

        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(DialogueText dialogueText)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }

    private void EndConversation()
    {
        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        int maxVisibleChars = 0;

        dialogueTextMesh.text = p;
        dialogueTextMesh.maxVisibleCharacters = maxVisibleChars;

        foreach (char c in p.ToCharArray())
        {
            maxVisibleChars++;
            dialogueTextMesh.maxVisibleCharacters = maxVisibleChars;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        StopCoroutine(typeDialogueCoroutine);
        dialogueTextMesh.maxVisibleCharacters = p.Length;
        isTyping = false;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
}
