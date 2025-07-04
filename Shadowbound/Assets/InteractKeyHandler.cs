using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractKeyHandler : MonoBehaviour
{
    private Image _keyIcon;
    private string _currentScheme = "None";
    private bool _keyChanged = false;
    public enum Key
    {
        DropItem,
        Interact
    }

    private Key _key = Key.Interact;
    public Key KeyType
    {
        get => _key;
        set {
            _keyChanged = true;
            _key = value;
        }
    }
    void Awake()
    {
        _keyIcon = GetComponent<Image>();
    }

    void Update()
    {
        if ((_currentScheme == PlayerInput.Instance.CurrentScheme || PlayerInput.Instance.CurrentScheme == "None") && !_keyChanged)
            return;
        _keyChanged = false;
        _currentScheme = PlayerInput.Instance.CurrentScheme;
        InputAction action = _key switch
        {
            Key.Interact => PlayerInput.Instance.Controls.Player.Interact,
            Key.DropItem => PlayerInput.Instance.Controls.Player.DropItem,
            _ => null
        };
        InputBinding binding = action.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
        _keyIcon.sprite = InputSpritesByKey.Instance.GetSpriteFromBindingPath(binding.effectivePath);
    }
}
