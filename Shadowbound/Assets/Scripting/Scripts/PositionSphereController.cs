using System;
using UnityEngine;

public class PositionSphereController : MonoBehaviour
{
    private CharacterController _controller;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _maxDistance = 3f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _groundedGravity = -0.5f;

    private float _verticalVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.position = PossessionHandler.Instance.PossessedEntity.transform.position
            - PossessionHandler.Instance.PossessedEntity.transform.forward * 0.20f
            + Vector3.up * 0.15f;
    }

    private void Update()
    {
        Vector3 input = new Vector3(PlayerInput.Instance.MovementInput.x, 0, PlayerInput.Instance.MovementInput.z);
        Vector3 moveDir = Vector3.zero;

        if (input.sqrMagnitude > 0f)
        {
            moveDir = Camera.main.transform.TransformDirection(input);
            moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized;

            Vector3 futurePos = transform.position + moveDir * _speed * Time.unscaledDeltaTime;
            if (PossessionHandler.Instance.PossessedEntity != null &&
                Vector3.Distance(futurePos, PossessionHandler.Instance.PossessedEntity.transform.position) > _maxDistance)
            {
                moveDir = Vector3.zero;
            }
        }

        // Gravità
        if (_controller.isGrounded)
        {
            _verticalVelocity = _groundedGravity;
        }
        else
        {
            _verticalVelocity += _gravity * Time.unscaledDeltaTime;
        }

        Vector3 gravityVector = Vector3.up * _verticalVelocity;
        Vector3 finalMovement = (moveDir * _speed + gravityVector) * Time.unscaledDeltaTime;

        _controller.Move(finalMovement);
    }
}
