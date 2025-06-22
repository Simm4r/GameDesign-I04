using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionKeySpritehandler : MonoBehaviour
{
    public static InteractionKeySpritehandler Instance { get; private set; }
    [SerializeField] private Image _key;

    private string _currentScheme = "None";

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
        InputBinding confirm = PlayerInput.Instance.Controls.Player.ConfirmPosition.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
        _key.sprite = InputSpritesByKey.Instance.GetSpriteFromBindingPath(confirm.effectivePath);
    }
}
