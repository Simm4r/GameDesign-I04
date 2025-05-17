using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    private static Player instance;
    private PlayerInput input;
    private bool canMove = true;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded;
    private Transform cameraTransform;
    private Bounds bounds;
    private Vector3 normalVec = Vector3.up;
    private bool onSteepSlope = false;
    private Dash dashScript;
    private Possession possession;

    [SerializeField] private float moveSpeed = 1.25f; //1.25 walk, 3 speed
    [SerializeField] private float transitionTime = 0.2f;
    [SerializeField] private float maxMoveSpeed = 5f; 
    [SerializeField] private float terminalSpeed = -100f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private Animator animator;
    [SerializeField] private float hitOffset = 0.015f;
    [SerializeField] float speed;
    [SerializeField] bool collided;
    [SerializeField] float maxSlopeAngle = 50f;

    public static Player Instance
    {
        get
        {
            return instance;
        }
    }
    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; } 
    }
    public Bounds MeshBounds
    {
        get { return bounds; }
        set { bounds = value; }
    }
    public Vector3 NormalVec
    {
        get { return normalVec; }
        set { normalVec = value; }
    }

    public float HitOffset
    {
        get { return hitOffset; }
        set { hitOffset = value; }
    }
    public bool Collided
    {
        get { return collided; }
        set { collided = value; }
    }
    private void CheckGrounded()
    {
        RaycastHit hitInfo;
        isGrounded = Physics.SphereCast(transform.position, bounds.extents.x, Vector3.down, out hitInfo, bounds.extents.y);

        if (isGrounded)
        {
            if (normalVec == Vector3.up)
            {
                velocity.y = 0f;
            }

            float yPoint = hitInfo.point.y + bounds.extents.y;
            transform.position = new Vector3(transform.position.x, yPoint, transform.position.z);
            Vector3 oldNormal = normalVec;
            normalVec = Vector3.Lerp(hitInfo.normal, oldNormal, 0.5f);
            normalVec = Vector3.Normalize(normalVec);

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            float dot = Vector3.Dot(projectedVelocity.normalized, hitInfo.normal);

            float surfaceAngle = Vector3.Angle(hitInfo.normal, Vector3.up);

            if (dot < 0f)
            {
                onSteepSlope = surfaceAngle > maxSlopeAngle;
            }
            else
            {
                onSteepSlope = false;
            }
        }
        else
        {
            normalVec = Vector3.up;
        }
    }

    private void Sprint() {
        if (dashScript.IsDashing)
            return;

        if (input.Sprint) moveSpeed = 3.0f;
        else moveSpeed = 1.25f;
    }

    private void CheckCollision() {     
        if(velocity == Vector3.zero)
            return;

        if (dashScript.IsDashing)
            return;

        RaycastHit hitInfo;
        Vector3 direction = velocity.normalized;
        direction = Vector3.ProjectOnPlane(direction, normalVec).normalized;
        float margin = Vector3.Dot(bounds.extents, new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z)));
        Vector3 start = transform.position - direction * hitOffset * 2f;
        collided = Physics.SphereCast(start, bounds.extents.x, direction, out hitInfo, margin + hitOffset); 

        if(collided) {
            float surfaceAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
            if (surfaceAngle < maxSlopeAngle)
            {
                CheckGrounded();
            }
            float dotVelocity = Vector3.Dot(velocity, direction);
            velocity = Vector3.zero;
            
            transform.position = Vector3.Lerp(start, transform.position - margin * direction, 0.1f);
        }
    }

    private void Fall() {
        if(isGrounded)
            return;

        if (dashScript.IsDashing)
            return;

        if (velocity.y > terminalSpeed)
        {
            velocity.y = velocity.y - (gravity * Time.deltaTime);

            if (velocity.y <= terminalSpeed)
            {
                velocity.y = terminalSpeed;
            }
        }

        transform.position += new Vector3(0.0f, velocity.y * Time.deltaTime, 0.0f);
    }
    private float currentSpeed = 0f;
    private float currentVert = 0f;

    private void Move() {
        if (!canMove)
            return;
            
        if (!isGrounded)
            return;

        if (dashScript.IsDashing)
            return;
        
        if (onSteepSlope)
        {
            velocity = Vector3.zero;
            return;
        }
        if (input.MovementInput != Vector3.zero)
        {

            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * input.MovementInput.z + camRight * input.MovementInput.x;
            moveDir.Normalize();

            moveDir = Vector3.ProjectOnPlane(moveDir, normalVec);

            velocity.x = moveDir.x * moveSpeed;
            velocity.y = moveDir.y * moveSpeed;
            velocity.z = moveDir.z * moveSpeed;

            transform.position += velocity * Time.deltaTime;


            speed = Mathf.Clamp01(new Vector3(velocity.x, 0f, velocity.z).magnitude / maxMoveSpeed);

            Vector3 lookDirection = new Vector3(input.MovementInput.x, 0f, input.MovementInput.z);
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime / transitionTime);
            currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime / transitionTime);
            currentVert = Mathf.Lerp(currentVert, 1f, Time.deltaTime / transitionTime);
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime / transitionTime);
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime / transitionTime);
            currentVert = Mathf.Lerp(currentVert, 0f, Time.deltaTime / transitionTime);
        }

        if (animator != null) {
            animator.SetFloat("State", currentSpeed);
            animator.SetFloat("Vert", currentVert);
        }
    }

    void Awake() {
        Assert.IsNull(instance);
        instance = this;

        input = GetComponent<PlayerInput>();
        dashScript = GetComponent<Dash>();
        bounds = GetComponentInChildren<Renderer>().bounds;
        possession = GetComponent<Possession>();
        cameraTransform = Camera.main.transform;
    }



    void Update()
    {
        if(possession.IsPossessing) {
            velocity = Vector3.zero;
            animator.SetFloat("State", 0);
            animator.SetFloat("Vert", 0);
            return;
        }
            

        Sprint();
        CheckGrounded();
        CheckCollision();
        Fall();
        Move();
    }

    private void OnDrawGizmosSelected() {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;

        // ----- Ground Check SphereCast -----
        Vector3 groundStart = transform.position;
        Vector3 groundDir = Vector3.down;
        float groundDistance = bounds.extents.y;
        float radius = bounds.extents.x;

        // Disegna la direzione del cast
        Gizmos.DrawWireSphere(groundStart, radius);
        Gizmos.DrawLine(groundStart, groundStart + groundDir * groundDistance);
        Gizmos.DrawWireSphere(groundStart + groundDir * groundDistance, radius);

        if (Physics.SphereCast(groundStart, radius, groundDir, out RaycastHit groundHit, groundDistance)) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(groundHit.point, 0.05f);
        }

        // ----- Collision Check SphereCast -----
        if (velocity != Vector3.zero) {
            Gizmos.color = Color.cyan;

            Vector3 direction = velocity.normalized;
            direction = Vector3.ProjectOnPlane(direction, normalVec).normalized;

            float margin = Vector3.Dot(bounds.extents, new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z)));
            float distance = margin + hitOffset;

            Vector3 collisionStart = transform.position;

            Gizmos.DrawWireSphere(collisionStart, radius);
            Gizmos.DrawLine(collisionStart, collisionStart + direction * distance);
            Gizmos.DrawWireSphere(collisionStart + direction * distance, radius);

            if (Physics.SphereCast(collisionStart, radius, direction, out RaycastHit collisionHit, distance)) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(collisionHit.point, 0.05f);
            }
        }
    }

}
