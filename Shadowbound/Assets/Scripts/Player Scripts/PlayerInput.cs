using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private bool _sprintWithButton = true;
    private PlayerControls _controls;
    private Vector2 _moveInput;
    private bool _inPossession;
    private bool _sprintKeyPressed;
    private Vector2 _lookInput;
    public Vector2 LookInput => _lookInput;
    public Vector3 MovementInput => new Vector3(_moveInput.x, 0f, _moveInput.y);
    public bool InPossession
    {
        get => _inPossession;
        set => _inPossession = value;
    }

private bool _isAnalogSprinting = false;

    public bool Sprint
    {
        get
        {
            bool usingGamepad = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;

            if (usingGamepad)
            {
                if (_sprintWithButton)
                {
                    return _sprintKeyPressed;
                }
                else
                {
                    float thresholdEnter = 0.8f;
                    float thresholdExit = 0.6f;

                    if (!_isAnalogSprinting && _moveInput.magnitude >= thresholdEnter)
                        _isAnalogSprinting = true;
                    else if (_isAnalogSprinting && _moveInput.magnitude <= thresholdExit)
                        _isAnalogSprinting = false;

                    return _isAnalogSprinting;
                }
            }
            else
            {
                return _sprintKeyPressed; // da tastiera
            }
        }
    }

    public bool ShadowStep => !_inPossession && _controls.Player.ShadowStep.ReadValue<float>() > 0;
    public bool Possessing => !_inPossession && _controls.Player.Possession.ReadValue<float>() > 0;
    public bool ShadowVision => !_inPossession && _controls.Player.ShadowVision.ReadValue<float>() > 0;
    public bool Interact => _inPossession && _controls.Player.Interact.ReadValue<float>() > 0;
    public bool QuitPossession => _inPossession && _controls.Player.QuitPossession.ReadValue<float>() > 0;

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _controls.Player.Sprint.performed += ctx => _sprintKeyPressed = true;
        _controls.Player.Sprint.canceled += ctx => _sprintKeyPressed = false;

        _controls.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
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