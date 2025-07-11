using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuHandler : MonoBehaviour
{
    public static MenuHandler Instance { get; private set; }
    [SerializeField] private List<GameObject> _slots;
    [SerializeField] private List<GameObject> _levels;
    [SerializeField] private GameObject _highlightBar;
    private string _currentScheme = "None";
    private CanvasGroup _highlightCanvas;
    private int _actualSlot = 0;
    private int _actualLvSlot = 0;
    private bool _hoveringSomething = false;
    [SerializeField] private GameObject _lvSelectionMenu;
    [SerializeField] private GameObject _lvHighlightBar;
    [SerializeField] private AudioSource _source;
    private bool _inLvSelection = false;
    private bool _hoveringSomethingLv = false;
    private CanvasGroup _lvHighlightCanvas;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _highlightCanvas = _highlightBar.GetComponent<CanvasGroup>();
        _highlightCanvas.alpha = 0;
        _lvHighlightCanvas = _lvHighlightBar.GetComponent<CanvasGroup>();
        _lvHighlightCanvas.alpha = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (ScreenFadeController.Instance.IsFading)
            return;

        if (_inLvSelection)
        {
            if (_currentScheme != MenuInput.Instance.CurrentScheme)
            {
                _currentScheme = MenuInput.Instance.CurrentScheme;
                switch (_currentScheme)
                {
                    case "Gamepad":
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        _actualLvSlot = 0;
                        if (_highlightCanvas.alpha < 0.1f)
                            _highlightCanvas.alpha = 0.1f;
                        HighlightLvSlot();
                        break;
                    case "MouseKeyboard":
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        break;
                    default:
                        break;
                }
            }

            HandleLevelSelection();
            return;
        }
        if (_currentScheme != MenuInput.Instance.CurrentScheme)
        {
            _currentScheme = MenuInput.Instance.CurrentScheme;
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
        HandleMenu();
    }

    private void HandleLevelSelection()
    {
        switch (MenuInput.Instance.CurrentScheme)
        {
            case "Gamepad":
                HandleGamepadInputLv();
                break;
            case "MouseKeyboard":
                HandleMouseKeyboardInputLv();
                break;
            default:
                break;
        }
        if (MenuInput.Instance.Cancel)
        {
            _inLvSelection = false;
            _lvSelectionMenu.SetActive(false);
            _hoveringSomethingLv = false;
        }
    }

    private void HandleMouseKeyboardInputLv()
    {
        bool clickedOnSomething = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (!_hoveringSomethingLv && _lvHighlightCanvas.alpha > 0.0f)
            _lvHighlightCanvas.alpha = 0.0f;

        if (_hoveringSomethingLv && MenuInput.Instance.MouseConfirm)
        {
            ActivateSlotLv(_actualLvSlot);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        for (int i = 0; i < _levels.Count; i++)
        {
            RectTransform rect = _levels[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, MenuInput.Instance.MousePosition))
            {
                _hoveringSomethingLv = true;

                if (_actualLvSlot != i)
                {
                    _actualLvSlot = i;
                    HighlightLvSlot();
                }

                if (_lvHighlightCanvas.alpha < 0.1f)
                    _lvHighlightCanvas.alpha = 0.1f;

                if (MenuInput.Instance.MouseConfirm)
                {
                    clickedOnSomething = true;
                    ActivateSlotLv(_actualLvSlot);
                }        
                break;
            }
            if (!clickedOnSomething && MenuInput.Instance.MouseConfirm)
            {
                _inLvSelection = false;
                _lvSelectionMenu.SetActive(false);
            }
            _hoveringSomethingLv = false;
            
        }  
    }

    private void ActivateSlotLv(int i)
    {
        HUDAudioHandler.Instance.StartSound();
        switch (i)
        {
            case 0:
                ScreenFadeController.Instance.FadeToBlack();
                StartCoroutine(ChangeLevel("lv1"));
                break;
            case 1:
                ScreenFadeController.Instance.FadeToBlack();
                StartCoroutine(ChangeLevel("lv2"));
                break;
        }
    }

    IEnumerator ChangeLevel(string v)
    {
        while (_source.volume > 0)
        {
            _source.volume = Mathf.Clamp01(_source.volume - Time.deltaTime / 2f);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(v);
    }

    private void HandleGamepadInputLv()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (MenuInput.Instance.Down && _actualLvSlot < _levels.Count - 1)
        {
            _actualLvSlot++;
            HighlightLvSlot();
        }

        else if (MenuInput.Instance.Up && _actualLvSlot > 0)
        {
            _actualLvSlot--;
            HighlightLvSlot();
        }
        else if (MenuInput.Instance.Confirm)
            ActivateSlotLv(_actualLvSlot);   
    }

    private void HandleMenu()
    {
        switch (MenuInput.Instance.CurrentScheme)
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
        if (!_hoveringSomething && _highlightCanvas.alpha > 0.0f)
            _highlightCanvas.alpha = 0.0f;

        if (_hoveringSomething && MenuInput.Instance.MouseConfirm)
        {
            ActivateSlot(_actualSlot);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            RectTransform rect = _slots[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, MenuInput.Instance.MousePosition))
            {
                _hoveringSomething = true;

                if (_actualSlot != i)
                {
                    _actualSlot = i;
                    HighlightSlot();
                }

                if (_highlightCanvas.alpha < 0.1f)
                    _highlightCanvas.alpha = 0.1f;

                if (MenuInput.Instance.MouseConfirm)
                    ActivateSlot(_actualSlot);
                break;
            }
            _hoveringSomething = false;
        }
    }

    private void HandleGamepadInput()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (MenuInput.Instance.Down && _actualSlot < _slots.Count - 1)
        {
            _actualSlot++;
            HighlightSlot();
        }

        else if (MenuInput.Instance.Up && _actualSlot > 0)
        {
            _actualSlot--;
            HighlightSlot();
        }
        else if (MenuInput.Instance.Confirm)
            ActivateSlot(_actualSlot);
    }

    private void HighlightSlot()
    {
        _highlightBar.transform.position = _slots[_actualSlot].transform.position;
    }

    private void HighlightLvSlot()
    {
        _lvHighlightBar.transform.position = _levels[_actualLvSlot].transform.position;
    }
    private void ActivateSlot(int i)
    {
        HUDAudioHandler.Instance.StartSound();
        Debug.Log(i);
        switch (i)
        {
            case 0:
                ScreenFadeController.Instance.FadeToBlack();
                StartCoroutine(PlayDemo());
                break;
            case 1:
                _lvSelectionMenu.SetActive(true);
                _inLvSelection = true;
                switch (_currentScheme)
                {
                    case "Gamepad":
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        _actualLvSlot = 0;
                        if (_lvHighlightCanvas.alpha != 0.1f)
                            _lvHighlightCanvas.alpha = 0.1f;
                        HighlightLvSlot();
                        break;
                    case "MouseKeyboard":
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        break;
                    default:
                        break;
                }
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

    IEnumerator PlayDemo()
    {
        while (_source.volume > 0)
        {
            _source.volume = Mathf.Clamp01(_source.volume - Time.deltaTime / 2f);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("intro");
    }
}
