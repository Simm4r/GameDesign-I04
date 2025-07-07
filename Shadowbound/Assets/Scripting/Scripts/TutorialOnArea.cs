using System.Linq;
using UnityEngine;

public class TutorialOnArea : MonoBehaviour
{
    [SerializeField] private string _tag;
    [SerializeField] private Vector3 _boxExtents;
    [SerializeField] private float _minCheckDistance = 1.0f;
    [SerializeField] private TutorialHandler.TutorialPage _tutorialPage;

    void Update()
    {
        var player = PlayerInput.Instance.InPossession ? PossessionHandler.Instance.PossessedEntity : Player.Instance.gameObject;
        if (Vector3.Distance(player.transform.position, transform.position) > _minCheckDistance)
            return;

        if (TutorialHandler.Instance.GetActualState() == "Idle")
        {
            Player.Instance.Tutorial = true;
            HandleTutorialPage();
            Collider[] hits = Physics.OverlapBox(transform.position + Vector3.up * 0.3f, _boxExtents, Quaternion.identity);
            hits = hits.Where(h => h.gameObject == player).ToArray();

            if (hits.Length == 0)
                return;
        }
        else if (TutorialHandler.Instance.GetActualState() == "OnScreen")
        {
            if (PlayerInput.Instance.DialogueNext)
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
            case TutorialHandler.TutorialPage.SS:
                TutorialHandler.Instance.Hide();
                enabled = false;
                break;
            case TutorialHandler.TutorialPage.CP:
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
            case TutorialHandler.TutorialPage.SS:
                TutorialHandler.Instance.SetActualTutorial(TutorialHandler.TutorialPage.SS);
                break;
            case TutorialHandler.TutorialPage.CP:
                TutorialHandler.Instance.SetActualTutorial(TutorialHandler.TutorialPage.CP);
                break;
        }
        TutorialHandler.Instance.Show();
    }

}
