using TMPro;
using UnityEngine;

public class NotificationBar : MonoBehaviour
{
    public static NotificationBar Instance { get; private set; }
    private CanvasGroup _canvasGroup;
    private bool _isBlinking = false;
    private TextMeshProUGUI _text;
    private float _blinkTimer;
    private float[] _blinkPhases = new float[4];
    private float _phaseDuration;
    [SerializeField] private float totalBlinkDuration = 3f;

    public bool IsBlinking
    {
        get => _isBlinking;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _canvasGroup = GetComponent<CanvasGroup>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void StartBlink()
    {
        _blinkTimer = 0f;
        _isBlinking = true;
        _phaseDuration = totalBlinkDuration / 4f;
        _canvasGroup.alpha = 0f;

    }

        void Update()
    {
        if (!_isBlinking)
            return;

        _blinkTimer += Time.deltaTime;

        if (_blinkTimer >= totalBlinkDuration)
        {
            _isBlinking = false;
            _canvasGroup.alpha = 0f;
            return;
        }

        float phase = _blinkTimer / _phaseDuration;
        float t = _blinkTimer % _phaseDuration / _phaseDuration; 

        if (phase < 1f)
        {
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
        }
        else if (phase < 2f)
        {
            _canvasGroup.alpha = Mathf.Lerp(1f, 0.5f, t);
        }
        else if (phase < 3f)
        {
            _canvasGroup.alpha = Mathf.Lerp(0.5f, 1f, t);
        }
        else
        {
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
        }
    }
}

