using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInput : MonoBehaviour
{
    public static MenuInput Instance { get; private set; }
    private PlayerControls _controls;
    private string _currentScheme = "None";
    private Vector2 _mousePosition;
    public Vector2 MousePosition => _mousePosition;
    public bool Up => _controls.Menu.Up.triggered;
    public bool Down => _controls.Menu.Down.triggered;
    public bool Confirm => _controls.Menu.Confirm.triggered;
    public bool Cancel => _controls.Menu.Cancel.triggered;
    public bool MouseConfirm => _controls.Menu.MouseConfirm.triggered;
    public string CurrentScheme => _currentScheme;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _controls = new PlayerControls();

        _controls.Menu.Up.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Menu.Up.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Menu.Down.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Menu.Down.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Menu.Confirm.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Menu.Confirm.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Menu.Cancel.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Menu.Cancel.canceled += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };

        _controls.Menu.Mouse.performed += ctx =>
        {
            _mousePosition = ctx.ReadValue<Vector2>();
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Player.Mouse.canceled += ctx =>
        {
            _mousePosition = Vector2.zero;
            UpdateCurrentScheme(ctx.control.device);
        };
        
        _controls.Menu.MouseConfirm.performed += ctx =>
        {
            UpdateCurrentScheme(ctx.control.device);
        };
        _controls.Menu.MouseConfirm.canceled += ctx =>
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
