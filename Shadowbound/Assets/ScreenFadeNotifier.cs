using UnityEngine;

public class ScreenFadeNotifier : MonoBehaviour
{
    public static ScreenFadeNotifier Instance { get; private set; }
    private bool _inAnimation = false;
    public bool InAnimation
    {
        get => _inAnimation;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void StartAnimation()
    {
        _inAnimation = true;
    }

    public void StopAnimation()
    {
        _inAnimation = false;
    }
}
