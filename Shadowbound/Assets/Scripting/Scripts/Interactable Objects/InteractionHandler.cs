using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private GameObject _player = null;
    private Interactable _object;
    [SerializeField] private float _triggerDistance = 1f;
    [SerializeField] private string _text;
    private enum State
    {
        Added,
        Removed,
        None
    }

    private State _actualState = State.None;

    public string Text
    {
        get => _text;
        set => _text = value;
    }
    public GameObject Player
    {
        set { _player = value; }
        get => _player;
    }
    void Awake()
    {
        _object = GetComponent<Interactable>();
    }

    void Update()
    {

        if (!_object.CanInteract || _player == null)
        {
            _actualState = State.Removed;
            InteractionBarHandler.Instance.RemoveInteraction(this);
            return;
        }


        float distance = Vector3.Distance(_player.transform.position, transform.position);

        if (distance > _triggerDistance || PlayerInput.Instance.Dying)
        {
            _actualState = State.Removed;
            InteractionBarHandler.Instance.RemoveInteraction(this);
            return;
        }

            _actualState = State.Added;
            InteractionBarHandler.Instance.AddInteraction(this);
    }

    public void Interact()
    {
        _object.Interact();
    }
}
