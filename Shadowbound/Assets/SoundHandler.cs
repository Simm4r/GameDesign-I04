using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _steps = new();
    [SerializeField] private AudioSource _source;
    public void ReproduceFootstep()
    {
        var index = Random.Range(0, _steps.Count);
        _source.resource = _steps[index];
        _source.Stop();
        _source.Play();
    }
}
