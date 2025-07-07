using UnityEngine;

public class AfterDialogueDisable : MonoBehaviour
{
    [SerializeField] private SingleDialogue _dialogue;


    void Update()
    {
        if (_dialogue.DialogueFinished)
            Destroy(gameObject);
    }
}
