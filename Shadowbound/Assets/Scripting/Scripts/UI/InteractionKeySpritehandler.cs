using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionKeySpritehandler : MonoBehaviour
{
    public static InteractionKeySpritehandler Instance { get; private set; }
    [SerializeField] private Image _keyIcon;
    private bool _inputChanged = false;
    public enum KeyType
    {
        Interact,
        QuitPossession,
        DialogueNext,
        None
    }
    private KeyType _keyType = KeyType.None;
    private string _currentScheme = "None";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        HandleSprite();
    }

    private void HandleSprite()
    {
        if ((_currentScheme == PlayerInput.Instance.CurrentScheme || PlayerInput.Instance.CurrentScheme == "None") && !_inputChanged)
            return;

        _inputChanged = false;
        InputBinding? confirm = null;
        switch (_keyType)
        {

            case KeyType.Interact:
                _currentScheme = PlayerInput.Instance.CurrentScheme;
                confirm = PlayerInput.Instance.Controls.Player.ConfirmPosition.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
                Debug.Log(confirm?.effectivePath);
                break;
            case KeyType.QuitPossession:
                _currentScheme = PlayerInput.Instance.CurrentScheme;
                confirm = PlayerInput.Instance.Controls.Player.QuitPossession.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
                break;
            case KeyType.DialogueNext:
                _currentScheme = PlayerInput.Instance.CurrentScheme;
                confirm = PlayerInput.Instance.Controls.Player.DialogueNext.bindings.FirstOrDefault(b => b.groups.Contains(_currentScheme));
                break;
            default:
                confirm = null;
                break;
        }
       
        _keyIcon.sprite = InputSpritesByKey.Instance.GetSpriteFromBindingPath(confirm?.effectivePath);
    }

    public void setKey(KeyType key)
    {
        _keyType = key;
        _inputChanged = true;
    }
}
