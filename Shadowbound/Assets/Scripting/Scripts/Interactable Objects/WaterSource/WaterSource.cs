using System.Linq;
using UnityEngine;

public class WaterSource : Interactable
{
    [SerializeField] private InteractionHandler _caller;
    private GameObject _possessedEntity;
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }
    [SerializeField] private AudioSource _source;

    public override void Interact()
    {
        GameObject torch = _possessedEntity.GetComponent<GuardStats>().Torch;
        Light torchLight = torch.GetComponentInChildren<Light>();

        if (!torchLight.enabled)
            return;

        _caller.Player = null;
        _canInteract = false;
        torchLight.enabled = false;
        var torchAudio = torch.GetComponentInChildren<AudioSource>(); 
        _source.Play();
        torchAudio.Stop();
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
            _caller.Player = null;
            _canInteract = false;
            return;
        }

        if (PossessionHandler.Instance.PossessedEntity != null && PossessionHandler.Instance.PossessedEntity.tag == "Possessable_Guard")
        {
            _possessedEntity = PossessionHandler.Instance.PossessedEntity;
            GameObject torch = _possessedEntity.GetComponent<GuardStats>().Torch;
            Light torchLight = torch.GetComponentInChildren<Light>();

            if (!torchLight.enabled)
            {
                _possessedEntity = null;
                return;
            }
            
            _caller.Player = PossessionHandler.Instance.PossessedEntity;
            _canInteract = true;
        }

    }
}