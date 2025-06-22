using UnityEngine;

public class SmallNotificationBarHandler : MonoBehaviour
{
    public static SmallNotificationBarHandler Instance { get; private set; }
    private CanvasGroup _canvas;
    void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
        _canvas = GetComponent<CanvasGroup>();
    }

    public void ShowBar()
    {
        _canvas.alpha = 1;
    }

    public void HideBar()
    {
        _canvas.alpha = 0;
    }
}
