using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    [SerializeField] private bool _sprintWithButton = true;
    private PlayerControls _controls;
    private Vector2 _moveInput;
    private bool _inPossession;
    private bool _sprintKeyPressed = false;
    private bool _dying = false;
    private Vector2 _lookInput;
    private Vector2 _mousePosition;
    public Vector2 MousePosition => _mousePosition;
    public Vector2 LookInput => _lookInput;
    public Vector3 MovementInput => new Vector3(_moveInput.x, 0f, _moveInput.y);
    public bool Dying
    {
        get => _dying;
        set => _dying = value;
    }
    public bool InPossession
    {
        get => _inPossession;
        set => _inPossession = value;
    }

    public PlayerControls Controls
    {
        get => _controls;
    }
    private string _currentScheme = "None";

    public string CurrentScheme => _currentScheme;

    public bool Sprint
    {
        get
        {

            if (_currentScheme == "Gamepad")
            {
                if (_sprintWithButton)
                {
                    return _sprintKeyPressed;
                }
                else
                {
                    Debug.Log("Sprint analogico");
                    return true;
                }
            }
            else
            {
                return _sprintKeyPressed;
            }
        }
    }

    public bool ShadowScreen => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && !_dying && !_inPossession && !Player.Instance.Reading && !Player.Instance.InDialogue && _controls.Player.ShadowScreen.ReadValue<float>() > 0;
    public bool Possessing => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && !_dying && !_inPossession && !Player.Instance.Reading && !Player.Instance.InDialogue && _controls.Player.Possession.ReadValue<float>() > 0;
    public bool ShadowVision => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && !_dying && !_inPossession && !Player.Instance.Reading && !Player.Instance.InDialogue && _controls.Player.ShadowVision.ReadValue<float>() > 0;
    public bool Interact => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && !_dying && !PossessionHandler.Instance.ChoosingPosition && !Player.Instance.Reading && !Player.Instance.InDialogue && _controls.Player.Interact.triggered;
    public bool QuitPossession => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && !Player.Instance.Reading && !Player.Instance.InDialogue && _inPossession && _controls.Player.QuitPossession.ReadValue<float>() > 0;
    public bool DialogueNext => !PauseHandler.Instance.InPause && (Player.Instance.InDialogue || Player.Instance.Reading || Player.Instance.Tutorial || Player.Instance.InCutscene) && _controls.Player.DialogueNext.triggered;
    public bool DropItem => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && !Player.Instance.Reading && !Player.Instance.InDialogue && !PossessionHandler.Instance.ChoosingPosition && _inPossession && _controls.Player.DropItem.triggered;
    public bool InteractUp => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && !Player.Instance.Reading && !PossessionHandler.Instance.ChoosingPosition && !Player.Instance.InDialogue && _controls.Player.InteractUp.triggered;
    public bool InteractDown => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && !Player.Instance.Reading && !PossessionHandler.Instance.ChoosingPosition && !Player.Instance.InDialogue && _controls.Player.InteractDown.triggered;
    public bool ConfirmPosition => !Player.Instance.InCutscene && !Player.Instance.Tutorial && !PauseHandler.Instance.InPause && PossessionHandler.Instance.ChoosingPosition && _controls.Player.ConfirmPosition.triggered;
    public bool Pause => !Player.Instance.InCutscene && !PauseHandler.Instance.InPause && _controls.Player.Pause.triggered;
    public bool PauseQuit => PauseHandler.Instance.InPause && _controls.Player.PauseQuit.triggered;
    public bool PauseUp => PauseHandler.Instance.InPause && _controls.Player.PauseUp.triggered;
    public bool PauseDown => PauseHandler.Instance.InPause && _controls.Player.PauseDown.triggered;
    public bool PauseConfirm => PauseHandler.Instance.InPause && _controls.Player.PauseConfirm.triggered;
    public bool MouseConfirm => _controls.Player.MouseConfirm.triggered;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        _controls = new PlayerControls();

        _controls.Player.Move.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.Move.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.Sprint.performed += ctx =>
        {
            _sprintKeyPressed = true;
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.Sprint.canceled += ctx =>
        {
            _sprintKeyPressed = false;
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.Look.performed += ctx =>
        {
            if (Player.Instance.InCutscene || Player.Instance.Tutorial || PauseHandler.Instance.InPause || Player.Instance.InDialogue || Player.Instance.Reading || Camera.main?.GetComponent<ThirdPersonCamera>().rotationSpeed != 10f)
            {
                _lookInput = Vector2.zero;
                return;
            }
            _lookInput = _currentScheme == "Gamepad" ? ctx.ReadValue<Vector2>() * Time.unscaledDeltaTime : ctx.ReadValue<Vector2>();
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.Look.canceled += ctx =>
        {
            if (Player.Instance.InCutscene || Player.Instance.Tutorial || PauseHandler.Instance.InPause || Player.Instance.InDialogue || Player.Instance.Reading)
            {
                _lookInput = Vector2.zero;
                return;
            }
            _lookInput = Vector2.zero;
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.Mouse.performed += ctx =>
        {
            _mousePosition = ctx.ReadValue<Vector2>();
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.Mouse.canceled += ctx =>
        {
            _mousePosition = Vector2.zero;
            UpdateCurrentScheme(ctx.control.device);
        };
        //ShadowScreen event register
        _controls.Player.ShadowScreen.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.ShadowScreen.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        //Possession event register
        _controls.Player.Possession.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.Possession.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        //ShadowVision event register
        _controls.Player.ShadowVision.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.ShadowVision.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        //Interact event register
        _controls.Player.Interact.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.Interact.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        //Possession event register
        _controls.Player.QuitPossession.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.QuitPossession.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        //Dialogue Button event register
        _controls.Player.DialogueNext.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.DialogueNext.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        //DropItem Button event register
        _controls.Player.DropItem.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.DropItem.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.InteractUp.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.InteractUp.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.InteractDown.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.InteractDown.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.ConfirmPosition.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.ConfirmPosition.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.Pause.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.Pause.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.PauseUp.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.PauseUp.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.PauseDown.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.PauseDown.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.PauseDown.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.PauseDown.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.PauseQuit.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.PauseQuit.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.PauseConfirm.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.PauseConfirm.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.MouseConfirm.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.MouseConfirm.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
    }

    private void Update()
    {
        if (Player.Instance.InCutscene || Player.Instance.Tutorial || ScreenFadeController.Instance.IsFading || PauseHandler.Instance.InPause || Player.Instance.InDialogue || Player.Instance.Reading || _dying || (Camera.main != null && Camera.main.GetComponent<ThirdPersonCamera>().rotationSpeed != 10f))
        {
            _moveInput = Vector2.zero;
            return;
        }

        Vector2 inputValue = _controls.Player.Move.ReadValue<Vector2>();
        ProcessMoveInput(inputValue);
    }

    private void ProcessMoveInput(Vector2 inputValue)
    {
        if (_sprintWithButton)
        {
            _moveInput = inputValue;
        }
        else if (_currentScheme == "Gamepad")
        {
            float mag = inputValue.magnitude;
            float smoothedMag = Mathf.Pow(mag, 2);
            _moveInput = mag < 0.3f ? inputValue.normalized * 0.3f : inputValue.normalized * smoothedMag;
        }
        else
        {
            _moveInput = inputValue;
        }
    }

    private void UpdateCurrentScheme(InputDevice device)
    {
        string newScheme;

        if (device is Gamepad)
        {
            if (_currentScheme == "Gamepad")
                return;
            newScheme = "Gamepad";
        }

        else if (device is Keyboard || device is Mouse)
        {
            if (_currentScheme == "MouseKeyboard")
                return;
            newScheme = "MouseKeyboard";
        }

        else
        {
            if (_currentScheme == "None")
                return;
            newScheme = "None";
        }

        _currentScheme = newScheme;

        Debug.Log(_currentScheme);
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
    
}