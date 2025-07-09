using System;
using KinematicCharacterController;
using UnityEngine;

public class ObjectMoveSound : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    private bool _itsMe = false;
    void Update()
    {
        if (PossessionHandler.Instance.PossessedEntity == gameObject)
            _itsMe = true;
        else
            _itsMe = false;

        HandleSound();
    }

    private void HandleSound()
    {
        if (!_source.isPlaying && _itsMe && PlayerInput.Instance.MovementInput != Vector3.zero && !PossessionHandler.Instance.ChoosingPosition)
            _source.Play();
        else if(PlayerInput.Instance.MovementInput == Vector3.zero || !_itsMe || PossessionHandler.Instance.ChoosingPosition)
            _source.Stop();
    }
}
