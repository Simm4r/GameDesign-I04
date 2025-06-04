using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private InputSpritesByKey _keyIcons;
    private Image _sprite;
    
    private string _currentScheme = "None";
    private string _gameTag;
    void Awake()
    {
        _gameTag = transform.parent.tag;
        _sprite = GetComponent<Image>();
    }
    void Update()
    {
        if (_currentScheme == PlayerInput.Instance.CurrentScheme)
            return;
       
        _currentScheme = PlayerInput.Instance.CurrentScheme;
        InputBinding bindingForScheme;

        switch (_gameTag)
        {
            case "Possession_Icon":
                bindingForScheme = PlayerInput.Instance.Controls.Player.Possession.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
                _sprite.sprite = _keyIcons.GetSpriteFromBindingPath(bindingForScheme.effectivePath);
                break;

            case "ShadowStep_Icon":
                bindingForScheme = PlayerInput.Instance.Controls.Player.ShadowStep.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
                _sprite.sprite = _keyIcons.GetSpriteFromBindingPath(bindingForScheme.effectivePath);
                break;

            case "ShadowVision_Icon":
                bindingForScheme = PlayerInput.Instance.Controls.Player.ShadowVision.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
                _sprite.sprite = _keyIcons.GetSpriteFromBindingPath(bindingForScheme.effectivePath);
                break;
        }
    }
}
