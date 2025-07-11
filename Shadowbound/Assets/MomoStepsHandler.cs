using System.Collections.Generic;
using UnityEngine;

public class MomoStepsHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private List<AudioClip> _steps;
    public void MomoStep()
    {
        if (PlayerInput.Instance.InPossession || Player.Instance.InDialogue || Player.Instance.InCutscene || Player.Instance.Reading || PlayerInput.Instance.MovementInput == Vector3.zero)
        {
            _source.Stop();
            return;
        }

        var index = Random.Range(0, _steps.Count);
        _source.Stop();
        _source.resource = _steps[index];
        _source.Play();
    }
}
