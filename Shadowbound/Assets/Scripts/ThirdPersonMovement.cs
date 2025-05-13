using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float gravity = -9.81f;
    public float rotationSpeed = 8f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;

    private float flowVert = 0f;
    private float flowState = 0f;
    private float smoothing = 5f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float runTrigger = Input.GetAxis("Run");
        bool isRunning = runTrigger > 0.5f;

        Vector2 input = new Vector2(h, v).normalized;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        float targetVert = 0f;

        if (input.magnitude >= 0.1f)
        {
            // Calculate movement relative to camera
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            targetVert = moveDir.magnitude;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animate
        flowVert = Mathf.MoveTowards(flowVert, targetVert, smoothing * Time.deltaTime);
        flowState = Mathf.MoveTowards(flowState, isRunning ? 1f : 0f, smoothing * Time.deltaTime);
        animator.SetFloat("Vert", flowVert);
        animator.SetFloat("State", flowState);
    }
}
