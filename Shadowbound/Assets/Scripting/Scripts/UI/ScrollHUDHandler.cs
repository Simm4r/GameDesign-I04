using TMPro;
using UnityEngine;

public class ScrollHUDHandler : MonoBehaviour
{
    public static ScrollHUDHandler Instance { get; private set; }
    private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _text;
    private float _alpha = 0;
    private enum State
    {
        Idle,
        Showing,
        Fading,
        OnScreen
    }
    private State _state = State.Idle;

    void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = _alpha;
    }

    void Update()
    {
        switch (_state)
        {
            case State.Showing:
                _alpha += 2 * Time.deltaTime;
                _alpha = Mathf.Clamp01(_alpha);
                _canvasGroup.alpha = _alpha;
                if (_alpha == 1)
                    _state = State.OnScreen;
                break;
            case State.Fading:
                _alpha -= 2 * Time.deltaTime;
                _alpha = Mathf.Clamp01(_alpha);
                _canvasGroup.alpha = _alpha;
                if (_alpha == 0)
                    _state = State.Idle;
                break;
            case State.OnScreen:
                if (PlayerInput.Instance.DialogueNext)
                    _state = State.Fading;
                break;
            case State.Idle:
                break;
        }
    }

    public void Show()
    {
        _state = State.Showing;
    }

    public void Hide()
    {
        _state = State.Fading;
    }


    public void SetText(string text)
    {
        _text.text = text;
    }

    public string GetActualState()
    {
        switch (_state)
        {
            case State.Showing:
                return "Showing";
            case State.Fading:
                return "Fading";
            case State.OnScreen:
                return "OnScreen";
            case State.Idle:
                return "Idle";
        }
        return "";
    }
}
