using TMPro;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    public static TutorialHandler Instance { get; private set; }
    private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject[] _possession;
    [SerializeField] private GameObject _shadowVision;
    [SerializeField] private GameObject _shadowScreen;
    [SerializeField] private GameObject _possessionEnhancement2 = null;
    [SerializeField] private GameObject _checkpoint;
    public enum TutorialPage
    {
        Pos1,
        Pos2,
        SV,
        SS,
        PosEn2,
        CP
    }
    private float _alpha = 0;
    private float _oldScaleTime = 1.0f;
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
        if (PauseHandler.Instance.InPause)
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
                    Player.Instance.Tutorial = false;
                    Time.timeScale = _oldScaleTime;
                    _state = State.Idle;
                }

                break;
            case State.OnScreen:
                break;
            case State.Idle:
                break;
        }
    }

    public void Show()
    {
        _oldScaleTime = Time.timeScale;
        Time.timeScale = 0;
        _state = State.Showing;
    }

    public void Hide()
    {
        _state = State.Fading;
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

    public void SetActualTutorial(TutorialPage page)
    {
        switch (page)
        {
            case TutorialPage.Pos1:
                _possession[1].SetActive(false);
                _checkpoint.SetActive(false);
                _possessionEnhancement2?.SetActive(false);
                _shadowScreen.SetActive(false);
                _shadowVision.SetActive(false);
                _possession[0].SetActive(true);
                break;
            case TutorialPage.Pos2:
                _shadowScreen.SetActive(false);
                _checkpoint.SetActive(false);
                _possessionEnhancement2?.SetActive(false);
                _shadowVision.SetActive(false);
                _possession[0].SetActive(false);
                _possession[1].SetActive(true);
                break;
            case TutorialPage.SV:
                _possession[1].SetActive(false);
                _checkpoint.SetActive(false);
                _possessionEnhancement2?.SetActive(false);
                _shadowScreen.SetActive(false);
                _possession[0].SetActive(false);
                _shadowVision.SetActive(true);
                break;
            case TutorialPage.SS:
                _possession[1].SetActive(false);
                _checkpoint.SetActive(false);
                _possessionEnhancement2?.SetActive(false);
                _possession[0].SetActive(false);
                _shadowVision.SetActive(false);
                _shadowScreen.SetActive(true);
                break;
            case TutorialPage.PosEn2:
                _possession[1].SetActive(false);
                _checkpoint.SetActive(false);
                _possession[0].SetActive(false);
                _shadowVision.SetActive(false);
                _shadowScreen.SetActive(false);
                _possessionEnhancement2.SetActive(true);
                break;
            case TutorialPage.CP:
                _possession[1].SetActive(false);  
                _possession[0].SetActive(false);
                _shadowVision.SetActive(false);
                _shadowScreen.SetActive(false);
                _possessionEnhancement2.SetActive(false);
                _checkpoint.SetActive(true);
                break;
        }
    }
}
