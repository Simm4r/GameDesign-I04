using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }
    private CanvasGroup _group;
    private float _alpha = 0.0f;
    private float _highlightAlpha = 0.0f;
    [SerializeField] private List<GameObject> _slots;
    [SerializeField] private GameObject _highlightBar;
    private string _currentScheme = "None";
    private CanvasGroup _highlightCanvas;
    private int _actualSlot = 0;
    private bool hoveringSomething = false;
    private float _oldScaleTime = 0.0f;
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
        _group = GetComponent<CanvasGroup>();
        _group.alpha = 0;
        _highlightCanvas = _highlightBar.GetComponent<CanvasGroup>();
        _highlightCanvas.alpha = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (PauseHandler.Instance.InPause)
        {
            if (_currentScheme != PlayerInput.Instance.CurrentScheme && (_state != State.Fading || _state != State.Idle))
            {
                _currentScheme = PlayerInput.Instance.CurrentScheme;
                switch (_currentScheme)
                {
                    case "Gamepad":
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        _actualSlot = 0;
                        if (_highlightCanvas.alpha < 0.1f)
                            _highlightCanvas.alpha = 0.1f;
                        HighlightSlot();
                        break;
                    case "MouseKeyboard":
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        break;
                    default:
                        break;
                }
            }
            switch (_state)
            {
                case State.Showing:
                    _alpha += 2 * Time.unscaledDeltaTime;
                    _alpha = Mathf.Clamp01(_alpha);
                    _highlightAlpha += 2 * Time.unscaledDeltaTime / 10;
                    _highlightAlpha = Mathf.Clamp(_highlightAlpha, 0.0f, 0.1f);
                    _group.alpha = _alpha;
                    _highlightCanvas.alpha = _highlightAlpha;
                    if (_alpha == 1)
                        _state = State.OnScreen;
                    break;
                case State.Fading:
                    _alpha -= 2 * Time.unscaledDeltaTime;
                    _alpha = Mathf.Clamp01(_alpha);
                    _highlightAlpha -= 2 * Time.unscaledDeltaTime / 10;
                    _highlightAlpha = Mathf.Clamp(_highlightAlpha, 0.0f, 0.1f);
                    _group.alpha = _alpha;
                    _highlightCanvas.alpha = _highlightAlpha;
                    if (_alpha == 0)
                    {
                        _state = State.Idle;
                        PauseHandler.Instance.InPause = false;
                        Time.timeScale = _oldScaleTime;
                    }

                    break;
                case State.OnScreen:
                    HandlePauseMenu();
                    break;
                case State.Idle:
                    break;
            }
        }
        else
        {
            if (Cursor.visible != false)
                Cursor.visible = false;

            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked; 
        }
            
    }

    private void HandlePauseMenu()
    {
        if (PlayerInput.Instance.PauseQuit)
        {
            Hide();
        }
            

        switch (PlayerInput.Instance.CurrentScheme)
        {
            case "Gamepad":
                HandleGamepadInput();
                break;
            case "MouseKeyboard":
                HandleMouseKeyboardInput();
                break;
            default:
                break;
        }
    }

    private void HandleMouseKeyboardInput()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (!hoveringSomething && _highlightCanvas.alpha > 0.0f)
            _highlightCanvas.alpha = 0.0f;

        if (hoveringSomething && PlayerInput.Instance.MouseConfirm)
        {
            ActivateSlot(_actualSlot);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
            
        for (int i = 0; i < _slots.Count; i++)
        {
            RectTransform rect = _slots[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, PlayerInput.Instance.MousePosition))
            {
                hoveringSomething = true;

                if (_actualSlot != i)
                {
                    _actualSlot = i;
                    HighlightSlot();
                }

                if (_highlightCanvas.alpha < 0.1f)
                    _highlightCanvas.alpha = 0.1f;

                if (PlayerInput.Instance.MouseConfirm)
                    ActivateSlot(_actualSlot);
                break;
            }
            hoveringSomething = false;
        }
    }

    private void HandleGamepadInput()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (PlayerInput.Instance.PauseDown && _actualSlot < _slots.Count - 1)
        {
            _actualSlot++;
            HighlightSlot();
        }

        else if (PlayerInput.Instance.PauseUp && _actualSlot > 0)
        {
            _actualSlot--;
            HighlightSlot();
        }
        else if (PlayerInput.Instance.PauseConfirm)
            ActivateSlot(_actualSlot);
    }

    IEnumerator WaitFade()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Debug.Log("loadingscene");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    private void HighlightSlot()
    {
        _highlightBar.transform.position = _slots[_actualSlot].transform.position;
    }

    public void Show()
    {
        _actualSlot = 0;
        ShowHiglightBar();
        HighlightSlot();
        _oldScaleTime = Time.timeScale;
        Time.timeScale = 0.0f;
        _state = State.Showing;
    }

    public void Hide()
    {
        _state = State.Fading;
    }

    private void ActivateSlot(int i)
    {
        Debug.Log(i);
        switch (i)
        {
            case 0:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Hide();
                break;
            case 1:
                var anim = ScreenFadeController.Instance.gameObject.GetComponentInChildren<Animator>();
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                ScreenFadeController.Instance.FadeToBlack();
                StartCoroutine(WaitFade());
                break;
            case 2:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
            default:
                break;
        }
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

    void ShowHiglightBar() {
        Image bar = _highlightBar.GetComponent<Image>();
        Color color = bar.color;
        color.a = 20;
        bar.color = color;
    }
    void HideHiglightBar() {
        Image bar = _highlightBar.GetComponent<Image>();
        Color color = bar.color;
        color.a = 0;
        bar.color = color;
    }
}
