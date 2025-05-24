using KinematicCharacterController;
using UnityEngine;

public class GuardAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private KinematicCharacterMotor _motor;
    [SerializeField] private float _walkTreshold = 0.1f;
    [SerializeField] private float _sprintTreshold = 2.35f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _motor = GetComponent<KinematicCharacterMotor>();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool("isWalking", _motor.Velocity.magnitude > _walkTreshold);
        _animator.SetBool("isRunning", _motor.Velocity.magnitude > _sprintTreshold);
    }
}
