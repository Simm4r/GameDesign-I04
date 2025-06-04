using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private GameObject _dashTrail;
    [SerializeField] private RespawnPoint _actualCheckpoint;
    private bool _inDialogue = false;

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
        _characterController = GetComponent<CharacterController>();
        _dashTrail.SetActive(false);
    }

    private void HandleCharacterInputs()
    {
        if (_characterController == null)
            return;
        _characterController.SetInputs();
    }

    private void Update()
    {
        HandleCharacterInputs();
    }
}