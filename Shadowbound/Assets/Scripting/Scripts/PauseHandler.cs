using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    public static PauseHandler Instance { get; private set; }
    private bool _inPause = false;
    private List<AudioSource> _sources = new();
    public bool InPause
    {
        get => _inPause;
        set => _inPause = value;
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None).Where(obj => !obj.CompareTag("HUD") && !obj.CompareTag("GameManager")).ToList();
    }

    void Update()
    {
        if (PlayerInput.Instance.Pause && !ScreenFadeNotifier.Instance.InAnimation)
        {
            _inPause = true;
            StopAudio();
            PauseMenu.Instance.Show();
        }

        if (Time.timeScale != 0 && Time.timeScale != 1)
        {
            _sources.ForEach(source =>
            {
                if (!source.CompareTag("PossessionRing"))
                    source.pitch = Time.timeScale;
            });
        }
        else
            _sources.ForEach(source => source.pitch = 1);
    }

    public void StopAudio()
    {
        _sources.ForEach(source => source.Pause());
    }

    public void PlayAudio()
    {
        _sources.ForEach(source => source.UnPause());
    }
}
