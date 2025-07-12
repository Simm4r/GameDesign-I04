using System;
using System.Collections;
using KinematicCharacterController;
using UnityEngine;

public class RatDen : Interactable
{
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private AudioSource _source;
    private enum Direction
    {
        Left,
        right,
    };

    [SerializeField] private Direction _direction;
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    public override void Interact()
    {
        _caller.Player = null;
        _canInteract = false;
        ScreenFadeController.Instance.FadeToBlack();
        Camera.main.GetComponent<ThirdPersonCamera>().enabled = false;
        StartCoroutine(waitForFade());
    }


    IEnumerator waitForFade()
    {
        yield return new WaitForSeconds(0.5f);
        _source.Stop();
        _source.Play();
        yield return new WaitForSeconds(1.5f);
        Player.Instance.GetComponent<KinematicCharacterMotor>().SetPositionAndRotation(_exitPoint.position, Quaternion.LookRotation(_exitPoint.forward));
        Camera.main.GetComponent<ThirdPersonCamera>()?.ForceSetCamera(Player.Instance.transform.position, _direction == Direction.Left ? Player.Instance.transform.right : -Player.Instance.transform.right);
        ScreenFadeController.Instance.FadeFromBlack();
        Camera.main.GetComponent<ThirdPersonCamera>().enabled = true;
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
        {
            _canInteract = true;
        }
            
    }
}
