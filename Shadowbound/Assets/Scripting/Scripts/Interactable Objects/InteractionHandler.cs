using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private GameObject _player = null;
    private Interactable _object;
    [SerializeField] private float _triggerDistance = 1f;
    private string _currentScheme = "None";
    [SerializeField] private Canvas _interactionBar;
    [SerializeField] private Image _uiBindingSprite;

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
        if (PlayerInput.Instance.Interact)
            _object.Interact();
    }

    private void HandleSprite()
    {
        if (_currentScheme == PlayerInput.Instance.CurrentScheme || PlayerInput.Instance.CurrentScheme == "None")
            return;

        _currentScheme = PlayerInput.Instance.CurrentScheme;
        InputBinding bindingForScheme = PlayerInput.Instance.Controls.Player.Interact.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
        _uiBindingSprite.sprite = InputSpritesByKey.Instance.GetSpriteFromBindingPath(bindingForScheme.effectivePath);
    }
}
