using TMPro;
using UnityEngine;

public class ScrollHUDHandler : MonoBehaviour
{
    public static ScrollHUDHandler Instance { get; private set; }
    private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _text;
    private float _alpha = 0;
    private float _oldScaleTime = 1.0f;
    private AudioSource _source;
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
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = _alpha;
    }

    void Update()
    {
        if (PauseHandler.Instance.InPause || Player.Instance.Tutorial)
            return;
            
        switch (_state)
        {
            case State.Showing:
                _alpha += 2 * Time.unscaledDeltaTime;
                _alpha = Mathf.Clamp01(_alpha);
                _canvasGroup.alpha = _alpha;
                if (_alpha == 1)
                    _state = State.OnScreen;
                break;
            case State.Fading:
                _alpha -= 2 * Time.unscaledDeltaTime;
                _alpha = Mathf.Clamp01(_alpha);
                _canvasGroup.alpha = _alpha;
                if (_alpha == 0)
                {
                    Time.timeScale = _oldScaleTime;
                    _state = State.Idle;
                }

                break;
            case State.OnScreen:
                if (PlayerInput.Instance.DialogueNext)
                {
                    HUDAudioHandler.Instance.StartSound();
                    _state = State.Fading; 
                }
                break;
            case State.Idle:
                break;
        }
    }

    public void Show()
    {
        _source.Play();
        _oldScaleTime = Time.timeScale;
        Time.timeScale = 0;
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

    public void SetSource(AudioSource source)
    {
        _source = source;
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
