using UnityEngine;

public class AudioSourcePlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    private bool _played = false;

    void Play()
    {
        _played = true;
        _source.Stop();
        _source.Play();
    }

    void Awake()
    {
        if (!_source.isPlaying)
            Destroy(gameObject);
    }
}
