using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Transform _camera;
    [SerializeField] private GameObject _dashTrail;

    private void Awake()
    {
        Application.targetFrameRate = 20;
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