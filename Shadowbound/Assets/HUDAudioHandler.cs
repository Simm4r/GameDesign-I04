using UnityEngine;

public class HUDAudioHandler : MonoBehaviour
{
    public static HUDAudioHandler Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    [SerializeField] private AudioSource _source;
    public void StartSound()
    {
        _source.Stop();
        _source.Play();
    }
}
