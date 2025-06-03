using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private GameObject _player = null;
    private Interactable _object;
    [SerializeField] private float _triggerDistance = 1f;
    [SerializeField] private PlayerInput _input;
    private string _currentScheme = "None";
    [SerializeField] private Canvas _interactionBar;
    [SerializeField] private Image _uiBindingSprite;
    [SerializeField] private InputSpritesByKey _keyIcons;

    public GameObject Player
    {
        set { _player = value; }
    }
    void Awake()
    {
        _object = GetComponent<Interactable>();
    }

    void Update()
    {
        
        if (!_object.CanInteract || _player == null)
        {
            if (_interactionBar.enabled)
                _interactionBar.enabled = false;
            return;
        }


        float distance = Vector3.Distance(_player.transform.position, transform.position);

        if (distance > _triggerDistance)
        {
            if (_interactionBar.enabled)
                _interactionBar.enabled = false;
            return;
        }

        HandleSprite();
        if (!_interactionBar.enabled)
            _interactionBar.enabled = true;
        if (_input.Interact)
            _object.Interact();
    }

    private void HandleSprite()
    {
        if (_currentScheme == _input.CurrentScheme || _input.CurrentScheme == "None")
            return;

        _currentScheme = _input.CurrentScheme;
        InputBinding bindingForScheme = _input.Controls.Player.Interact.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
        _uiBindingSprite.sprite = _keyIcons.GetSpriteFromBindingPath(bindingForScheme.effectivePath);
    }
}
