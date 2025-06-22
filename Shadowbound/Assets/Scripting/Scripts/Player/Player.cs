using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private GameObject _dashTrail;
    [SerializeField] private RespawnPoint _actualCheckpoint;
    private bool _inDialogue = false;
    private bool _reading = false;

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
            return;
        }
        
        Instance = this;
        Application.targetFrameRate = 500;
        _dashTrail.SetActive(false);
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