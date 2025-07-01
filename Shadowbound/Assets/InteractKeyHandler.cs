using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractKeyHandler : MonoBehaviour
{
    private Image _keyIcon;
    private string _currentScheme = "None";

    void Awake()
    {
        _keyIcon = GetComponent<Image>();
    }

    void Update()
    {
        if (_currentScheme == PlayerInput.Instance.CurrentScheme || PlayerInput.Instance.CurrentScheme == "None")
            return;

        _currentScheme = PlayerInput.Instance.CurrentScheme;
        InputBinding binding = PlayerInput.Instance.Controls.Player.Interact.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
        _keyIcon.sprite = InputSpritesByKey.Instance.GetSpriteFromBindingPath(binding.effectivePath);
    }
}
