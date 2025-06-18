using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionBarKeyHandler : MonoBehaviour
{
    public static InteractionBarKeyHandler Instance { get; private set; }
    private string _currentScheme = "None";
    [SerializeField] private Image _interactSprite;
    [SerializeField] private Image _upSprite;
    [SerializeField] private Image _downSprite;

    void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
    }
    void Update()
    {
        HandleSprite();
    }
    private void HandleSprite()
    {
        if (_currentScheme == PlayerInput.Instance.CurrentScheme || PlayerInput.Instance.CurrentScheme == "None")
            return;

        _currentScheme = PlayerInput.Instance.CurrentScheme;
        InputBinding interact = PlayerInput.Instance.Controls.Player.Interact.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
        InputBinding up = PlayerInput.Instance.Controls.Player.InteractUp.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
        InputBinding down = PlayerInput.Instance.Controls.Player.InteractDown.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
        _interactSprite.sprite = InputSpritesByKey.Instance.GetSpriteFromBindingPath(interact.effectivePath);
        _upSprite.sprite = InputSpritesByKey.Instance.GetSpriteFromBindingPath(up.effectivePath);
        _downSprite.sprite = InputSpritesByKey.Instance.GetSpriteFromBindingPath(down.effectivePath);
    }

}
