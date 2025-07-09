using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.AI;

public class SoundHandler : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _steps = new();
    [SerializeField] private AudioSource _source;
    public void ReproduceFootstep()
    {
        float distance = Vector3.Distance(MoveListener.Instance.transform.position, transform.position);
        Debug.Log(_source.maxDistance);
        Debug.Log("Vert " + gameObject + " " + GetComponent<Animator>().GetFloat("Vert"));
        if (Vector3.Distance(MoveListener.Instance.transform.position, transform.position) > _source.maxDistance || GetComponent<Animator>().GetFloat("Vert") < 0.1f || Player.Instance.InCutscene || Player.Instance.InDialogue || PauseHandler.Instance.InPause || Player.Instance.Reading)
        {
            _source.Stop();
            return;
        }
        Debug.Log("Sono qui" + gameObject);
        var index = Random.Range(0, _steps.Count);
        _source.resource = _steps[index];
        _source.Stop();
        _source.Play();
    }
}
