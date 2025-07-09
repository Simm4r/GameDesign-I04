using System;
using System.Collections.Generic;
using UnityEngine;

public class PotionDepot : Interactable
{
    public enum PotionType
    {
        Red,
        Blue,
        Green
    }
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    [SerializeField] private PotionType _type;
    [SerializeField] private List<GameObject> _potions;
    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private AudioSource _source;

    public override void Interact()
    {
        GuardStats stats = _caller.Player.GetComponent<GuardStats>();
        _caller.Player = null;
        _canInteract = false;
        SetStatus(stats);
        GameObject obj = _potions[0];
        _potions.RemoveAt(0);
        Destroy(obj);
        _source.Play();
    }

    private void SetStatus(GuardStats stats)
    {
        switch (_type)
        {
            case PotionType.Blue:
                stats.Status = GuardStats.GuardStatus.Sleepy;
                break;
            case PotionType.Red:
                stats.Status = GuardStats.GuardStatus.Scared;
                break;
            case PotionType.Green:
                stats.Status = GuardStats.GuardStatus.Fastened;
                break;
        }
    }

    void Update()
    {
        if (_potions.Count == 0)
        {
            _caller.Player = null;
            _canInteract = false;
            return;
        }
        if (!PlayerInput.Instance.InPossession)
        {
            _caller.Player = null;
            _canInteract = false;
            return;
        }

        if (PossessionHandler.Instance.PossessedEntity != null && PossessionHandler.Instance.PossessedEntity.tag == "Possessable_Guard" && PossessionHandler.Instance.PossessedEntity.GetComponent<GuardStats>().Status == GuardStats.GuardStatus.None)
        {
            _caller.Player = PossessionHandler.Instance.PossessedEntity;
            _canInteract = true;
        }
        else
        {
            _caller.Player = null;
            _canInteract = false;
        }
    }
}
