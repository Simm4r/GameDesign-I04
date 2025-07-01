using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerCutscene : MonoBehaviour
{
    [SerializeField] private GameObject _checkPoint;
    [SerializeField] private DialogueRepeatable _dialogue;
    void Update()
    {
        if (Player.Instance.ActualCheckPoint.position == _checkPoint.transform.position
        && Player.Instance.ActualCheckPoint.rotation == _checkPoint.transform.rotation
        && Player.Instance.ActualCheckPoint.scene == SceneManager.GetActiveScene().name)
        {
            StartCuscene.Instance.StartCutscene(null, _dialogue);
            enabled = false;
        }
    }
}
