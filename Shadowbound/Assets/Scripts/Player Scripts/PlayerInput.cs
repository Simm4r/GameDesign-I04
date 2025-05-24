using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    [Header("Movimento")]
    [SerializeField] private KeyCode _forwardKey = KeyCode.W;
    [SerializeField] private KeyCode _backwardKey = KeyCode.S;
    [SerializeField] private KeyCode _leftKey = KeyCode.A;
    [SerializeField] private KeyCode _rightKey = KeyCode.D;
    [SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;

    [Header("Abilità Momo")]
    [SerializeField] private KeyCode _shadowStepKey = KeyCode.Space;
    [SerializeField] private KeyCode _possessionKey = KeyCode.E;

    [Header("Abilità Posseduti")]
    [SerializeField] private KeyCode _interactionKey = KeyCode.E;
    private Vector3 _movementInput;
    private bool _inPossession = false;

    public Vector3 MovementInput => _movementInput;
    public bool InPossession
    {
        get { return _inPossession; }
        set { _inPossession = value; }
    }
    public bool Sprint => Input.GetKey(_sprintKey);

    public bool ShadowStep => !_inPossession && Input.GetKey(_shadowStepKey);

    public bool Possessing => !_inPossession && Input.GetKey(_possessionKey);

    public bool Interact => _inPossession && Input.GetKey(_interactionKey);
    private void Update()
    {
        GetInput();
    }

    private void GetInput() {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        if (Input.GetKey(_forwardKey)) z += 1f;
        if (Input.GetKey(_backwardKey)) z -= 1f;
        if (Input.GetKey(_rightKey)) x += 1f;
        if (Input.GetKey(_leftKey)) x -= 1f;

        _movementInput = new Vector3(x, y, z).normalized;
    }
    
}