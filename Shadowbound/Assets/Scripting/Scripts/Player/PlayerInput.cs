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

    public bool ShadowScreen =>!_dying && !_inPossession && !Player.Instance.InDialogue && _controls.Player.ShadowScreen.ReadValue<float>() > 0;
    public bool Possessing => !_dying && !_inPossession && !Player.Instance.InDialogue && _controls.Player.Possession.ReadValue<float>() > 0;
    public bool ShadowVision =>!_dying && !_inPossession && !Player.Instance.InDialogue && _controls.Player.ShadowVision.ReadValue<float>() > 0;
    public bool Interact => !_dying && !Player.Instance.InDialogue && _controls.Player.Interact.triggered;
    public bool QuitPossession => !Player.Instance.InDialogue && _inPossession && _controls.Player.QuitPossession.ReadValue<float>() > 0;
    public bool DialogueNext => Player.Instance.InDialogue && _controls.Player.DialogueNext.triggered;
    public bool DropItem => !Player.Instance.InDialogue && _inPossession && _controls.Player.DropItem.triggered;
    public bool InteractUp => !Player.Instance.InDialogue && _controls.Player.InteractUp.triggered;
    public bool InteractDown => !Player.Instance.InDialogue && _controls.Player.InteractDown.triggered;

    private void Awake()
    {
        if (Instance != null)
        {
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
            if (Player.Instance.InDialogue)
            {
                _lookInput = Vector2.zero;
                return;
            }
            _lookInput = ctx.ReadValue<Vector2>();
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.Look.canceled += ctx =>
        {
            if (Player.Instance.InDialogue)
            {
                _lookInput = Vector2.zero;
                return;
            }
            _lookInput = Vector2.zero;
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
    }
    
    private void Update()
    {
        if (Player.Instance.InDialogue || _dying)
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