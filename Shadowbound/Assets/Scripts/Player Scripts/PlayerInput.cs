using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    [Header("Movimento")]
    [SerializeField] private KeyCode forwardKey = KeyCode.W;
    [SerializeField] private KeyCode backWardKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;

    [Header("Abilità")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode shadowStepKey = KeyCode.Space;
    [SerializeField] private KeyCode possessionKey = KeyCode.E;

    private Vector3 movementInput;

    public Vector3 MovementInput => movementInput;

    public bool Sprint => Input.GetKey(sprintKey);

    public bool ShadowStep => Input.GetKey(shadowStepKey);

    public bool Possessing => Input.GetKey(possessionKey);

    private void Update()
    {
        GetInput();
    }

    private void GetInput() {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        if (Input.GetKey(forwardKey)) z += 1f;
        if (Input.GetKey(backWardKey)) z -= 1f;
        if (Input.GetKey(rightKey)) x += 1f;
        if (Input.GetKey(leftKey)) x -= 1f;

        movementInput = new Vector3(x, y, z).normalized;
    }
}