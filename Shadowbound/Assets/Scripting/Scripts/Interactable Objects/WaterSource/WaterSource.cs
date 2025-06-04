using System.Linq;
using UnityEngine;

public class WaterSource : Interactable
{
    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private PossessionHandler _possessionHandler;
    private GameObject _possessedEntity;
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }

    public override void Interact()
    {
        GameObject torch = _possessedEntity.GetComponent<GuardStats>().Torch;
        Light torchLight = torch.GetComponentInChildren<Light>();

        if (!torchLight.enabled)
            return;

        _canInteract = false;
        torchLight.enabled = false;

        var torchFire = torch.GetComponentsInChildren<ParticleSystem>();
        torchFire.ToList().ForEach(ps =>
        {
            var emission = ps.emission;
            emission.rateOverTime = 0.0f;
        });
    }

    void Update()
    {
        if (!PlayerInput.Instance.InPossession)
        {
            if (_canInteract != false)
                _canInteract = false;
            return;
        }

        if (_possessionHandler.PossessedEntity != null && _possessionHandler.PossessedEntity.tag == "Possessable_Guard")
        {
            _possessedEntity = _possessionHandler.PossessedEntity;
            GameObject torch = _possessedEntity.GetComponent<GuardStats>().Torch;
            Light torchLight = torch.GetComponentInChildren<Light>();

            if (!torchLight.enabled)
            {
                _possessedEntity = null;
                return;
            }
            
            _caller.Player = _possessionHandler.PossessedEntity;
            _canInteract = true;
        }

    }
}