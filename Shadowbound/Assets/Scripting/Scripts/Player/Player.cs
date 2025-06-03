using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Transform _camera;
    [SerializeField] private GameObject _dashTrail;
    [SerializeField] private GameObject _actualCheckpoint;
    private bool _inDialogue = false;

    public bool InDialogue
    {
        get { return _inDialogue; }
        set { _inDialogue = value; }
    }

    public GameObject ActualCheckPoint
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
        _input = GetComponent<PlayerInput>();
        _camera = Camera.main.transform;
        _characterController = GetComponent<CharacterController>();
        _dashTrail.SetActive(false);
    }

    private void HandleCharacterInputs()
    {
        _characterController.SetInputs(ref _input, ref _camera);
    }

    private void Update()
    {
        HandleCharacterInputs();
    }
}