using UnityEngine;

public class ExclamationMarkSound : MonoBehaviour
{
    [SerializeField] private AudioSource _source;

    void OnEnable()
    {
        _source.Stop();
        _source.Play();
    }

    void OnDisable()
    {
        _source.Stop();       
    }
}
