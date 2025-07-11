using UnityEngine;

public class HandleSmokeScale : MonoBehaviour
{
    private ParticleSystem _smoke;
    private bool _inPause = false;
    void Awake()
    {
        _smoke = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (_inPause != PauseHandler.Instance.InPause)
        {
            var main = _smoke.main;
            _inPause = PauseHandler.Instance.InPause;
            if (_inPause)
            {
                main.useUnscaledTime = false;
            }
            else
                main.useUnscaledTime = true;
        }
    }
}
