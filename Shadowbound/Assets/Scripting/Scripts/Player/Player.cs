using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private RespawnPoint _actualCheckpoint;
    private bool _inDialogue = false;
    private bool _reading = false;
    private bool _tutorial = false;
    private bool _inCutscene = false;
    private bool _inShadowStep = false;

    public bool InShadowStep
    {
        get => _inShadowStep;
        set => _inShadowStep = value;
    }

    public bool InCutscene
    {
        get => _inCutscene;
        set => _inCutscene = value;
    }

    public bool Tutorial
    {
        get => _tutorial;
        set => _tutorial = value;
    }

    public bool Reading
    {
        get => _reading;
        set => _reading = value;
    }

    public bool InDialogue
    {
        get { return _inDialogue; }
        set { _inDialogue = value; }
    }

    public RespawnPoint ActualCheckPoint
    {
        get { return _actualCheckpoint; }
        set { _actualCheckpoint = value; }
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
        Application.targetFrameRate = 500;
    }

    private void HandleCharacterInputs()
    {
        if (PlayerController.Instance == null)
            return;
        PlayerController.Instance.SetInputs();
    }

    private void Update()
    {
        HandleCharacterInputs();
    }
}