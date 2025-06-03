using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private bool _sprintWithButton = true;
    private PlayerControls _controls;
    private Vector2 _moveInput;
    private bool _inPossession;
    private bool _sprintKeyPressed = false;
    private Vector2 _lookInput;
    public Vector2 LookInput => _lookInput;
    public Vector3 MovementInput => new Vector3(_moveInput.x, 0f, _moveInput.y);
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

    public bool ShadowStep => !_inPossession && !Player.Instance.InDialogue && _controls.Player.ShadowStep.ReadValue<float>() > 0;
    public bool Possessing => !_inPossession && !Player.Instance.InDialogue && _controls.Player.Possession.ReadValue<float>() > 0;
    public bool ShadowVision => !_inPossession && !Player.Instance.InDialogue && _controls.Player.ShadowVision.ReadValue<float>() > 0;
    public bool Interact => !Player.Instance.InDialogue && _controls.Player.Interact.ReadValue<float>() > 0;
    public bool QuitPossession => !Player.Instance.InDialogue && _inPossession && _controls.Player.QuitPossession.ReadValue<float>() > 0;
    public bool DialogueNext => Player.Instance.InDialogue && _controls.Player.DialogueNext.triggered;

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Player.Move.performed += ctx =>
        {
            if (Player.Instance.InDialogue)
            {
                _moveInput = Vector2.zero;
                return;
            }

            Vector2 inputValue = ctx.ReadValue<Vector2>();

            if (_sprintWithButton)
                _moveInput = inputValue;

            else if (_currentScheme == "Gamepad")
            {
                float mag = inputValue.magnitude;
                float smoothedMag = Mathf.Pow(mag, 2);
                _moveInput = smoothedMag < 0.3f ? inputValue.normalized * 0.3f : inputValue.normalized * smoothedMag;
            }
            else
                _moveInput = inputValue;

            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.Move.canceled += ctx =>
        {
            if (Player.Instance.InDialogue)
            {
                _moveInput = Vector2.zero;
                return;
            }
                
            _moveInput = Vector2.zero;
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
        //ShadowStep event register
        _controls.Player.ShadowStep.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Player.ShadowStep.canceled += ctx =>
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