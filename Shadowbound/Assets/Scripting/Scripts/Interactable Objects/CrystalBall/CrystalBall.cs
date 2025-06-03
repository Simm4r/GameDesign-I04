using UnityEngine;

public class CrystalBall : Interactable
{
    [SerializeField] private GameObject _respawnPosition;
    [SerializeField] private ParticleSystem _flame;
    [SerializeField] private PlayerInput _input;
    private bool _canInteract = true;
    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }

    public override void Interact()
    {
        _canInteract = false;
        Player.Instance.ActualCheckPoint = _respawnPosition;
    }

    void Update()
    {
        if (_input.InPossession)
        {
            if (_canInteract != false)
                _canInteract = false;
            return;
        }
            
        if (_flame.emission.rateOverTime.constant == 0 && !_canInteract)
            _canInteract = true;
    }
}
