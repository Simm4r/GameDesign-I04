using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class IsoPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    //public KeyCode runKey = KeyCode.LeftShift;

    [Header("Jumping")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    private float verticalVelocity = 0f;
    private bool isGrounded = false;

    private CharacterController controller;
    private Animator animator;

    private float flowVert = 0f;
    private float flowState = 0f;
    private float smoothing = 5f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float runTrigger = Input.GetAxis("Run");
        bool isRunning = runTrigger > 0.5f;
        bool isJumpPressed = Input.GetButtonDown("Jump");

        Vector2 input = new Vector2(h, v);

        float angle = 45f * Mathf.Deg2Rad;
        Vector2 rotatedInput = new Vector2(
            input.x * Mathf.Cos(angle) + input.y * Mathf.Sin(angle),
            -input.x * Mathf.Sin(angle) + input.y * Mathf.Cos(angle)
        );

        Vector3 moveDir = new Vector3(rotatedInput.x, 0f, rotatedInput.y).normalized;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        //Gravity & Jump Logic (just for my fun)
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -1f; 

        if (isGrounded && isJumpPressed)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDir * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        //Animation Parameters
        float targetVert = moveDir.magnitude;
        flowVert = Mathf.MoveTowards(flowVert, targetVert, smoothing * Time.deltaTime);
        flowState = Mathf.MoveTowards(flowState, isRunning ? 1f : 0f, smoothing * Time.deltaTime);

        animator.SetFloat("Vert", flowVert);
        animator.SetFloat("State", flowState);
        //animator.SetBool("IsJumping", !isGrounded); // optional, requires IsJumping param in Animator
    }
}
