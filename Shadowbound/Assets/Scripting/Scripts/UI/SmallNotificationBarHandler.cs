using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmallNotificationBarHandler : MonoBehaviour
{
    public static SmallNotificationBarHandler Instance { get; private set; }
    private CanvasGroup _canvas;
    [SerializeField] private TextMeshProUGUI _text;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _canvas = GetComponent<CanvasGroup>();
        HideBar();
    }

    public void ShowBar()
    {
        _canvas.alpha = 1;
    }

    public void HideBar()
    {
        _canvas.alpha = 0;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }
}
