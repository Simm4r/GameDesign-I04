using System.Collections;
using KinematicCharacterController;
using UnityEngine;

public class RatDen : Interactable
{
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private InteractionHandler _caller;
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    public override void Interact()
    {
        _canInteract = false;
        ScreenFadeController.Instance.FadeToBlack();
        StartCoroutine(waitForFade());
    }


    IEnumerator waitForFade()
    {
        yield return new WaitForSeconds(1.5f);
        Player.Instance.GetComponent<KinematicCharacterMotor>().SetPosition(_exitPoint.position);
        ScreenFadeController.Instance.FadeFromBlack();
    }

    void Update()
    {
        if (PlayerInput.Instance.InPossession)
        {
            if (_canInteract != false)
                _canInteract = false;

                _caller.Player = null;
            return;
        }

        _caller.Player = Player.Instance.gameObject;
        if (!ScreenFadeController.Instance.IsFading && !_canInteract)
            _canInteract = true;
    }
}
