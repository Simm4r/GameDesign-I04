using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cauldron : Interactable
{
    private bool _canInteract = false;
    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private List<ItemData> _recipie;
    [SerializeField] private ParticleSystem _visualEffect;
    [SerializeField] private StoneAnimator _stone;
    [SerializeField] private ParticleSystem _stoneGlow;
    private ItemData _guardItem;
    private bool _triggerStone = false;
    private Inventory _guardInventory;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    public override void Interact()
    {
        if (_guardItem == null || !_recipie.Contains(_guardItem))
        {

            _caller.Player = null;
            _canInteract = false;
            NotificationBar.Instance.SetText(_guardItem == null ? "No Ingredient selected" : "Wrong ingredient");
            NotificationBar.Instance.StartBlink();
            return;
        }
        _visualEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _visualEffect.Play(true);
        _guardInventory.RemoveItem(_guardItem);
        NotificationBar.Instance.SetText("Right ingredient");
        NotificationBar.Instance.StartBlink();
        _caller.Player = null;
        _canInteract = false;
        _recipie.Remove(_guardItem);
        _guardItem = null;
    }

        void Update()
    {
        if (_recipie.Count == 0 && _visualEffect.particleCount == 0)
        {
            if (!_triggerStone)
            {
                _triggerStone = true;
                _stone.StartAnimation();
                _stoneGlow.gameObject.SetActive(true);
            }
            return;
        }
        if (!PlayerInput.Instance.InPossession)
        {
            if (_canInteract != false)
                _canInteract = false;
            return;
        }

        if (PossessionHandler.Instance.PossessedEntity != null && PossessionHandler.Instance.PossessedEntity.tag == "Possessable_Guard")
        {
            _caller.Player = PossessionHandler.Instance.PossessedEntity;
            _guardInventory = PossessionHandler.Instance.PossessedEntity.GetComponent<Inventory>();
            if (_guardInventory.items.Count > 0 && _guardItem != _guardInventory.items[0].data)
                _guardItem = _guardInventory.items[0].data;
            if (NotificationBar.Instance.IsBlinking)
            {
                _caller.Player = null;
                _canInteract = false;
                return;
            }
            _canInteract = true;
        }

    }
}
