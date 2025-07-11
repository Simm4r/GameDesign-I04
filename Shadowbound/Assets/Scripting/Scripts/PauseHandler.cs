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

        try
        {
            if (Time.timeScale != 0 && Time.timeScale != 1)
            {
                foreach (var source in _sources)
                {
                    if (source == null) continue;
                    if (!source.CompareTag("PossessionRing"))
                        source.pitch = Time.timeScale;
                }
            }
            else
            {
                foreach (var source in _sources)
                {
                    if (source == null) continue;
                    source.pitch = 1;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Errore nell'aggiornamento del pitch audio: {e.Message}");
        }
    }

    public void StopAudio()
    {
        foreach (var source in _sources)
        {
            if (source == null) continue;
            try { source.Pause(); }
            catch (System.Exception e) { Debug.LogWarning($"Errore nel Pause(): {e.Message}"); }
        }
    }

    public void PlayAudio()
    {
        foreach (var source in _sources)
        {
            if (source == null) continue;
            try { source.UnPause(); }
            catch (System.Exception e) { Debug.LogWarning($"Errore nel UnPause(): {e.Message}"); }
        }
    }
}
