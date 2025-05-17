using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private GameObject catModel;
    [SerializeField] private GameObject dashModel;
    [SerializeField] private float dashDistance = 6f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float cooldownDuration = 2f;
    private PlayerInput input;
    private bool enableDash = true;
    private float cooldownTimer = 0f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private bool canMove;
    private Player player;

    public bool IsDashing
    {
        get { return isDashing; }
        set { isDashing = value; }
    }

    private void CheckCollision() {     
        if(player.Velocity == Vector3.zero)
            return;

        if (!isDashing)
            return;

        RaycastHit hitInfo;
        Vector3 direction = player.Velocity.normalized;
        direction = Vector3.ProjectOnPlane(direction, player.NormalVec).normalized;
        float margin = Vector3.Dot(player.MeshBounds.extents, new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z)));
        Vector3 start = transform.position - direction * player.HitOffset * 2f;
        player.Collided = Physics.SphereCast(start, player.MeshBounds.extents.x, direction, out hitInfo, margin + player.HitOffset);
        if (hitInfo.collider && hitInfo.collider.CompareTag("Passable"))
            return;
        if (player.Collided)
            {
                EndDash();
                float dotVelocity = Vector3.Dot(player.Velocity, direction);
                player.Velocity = Vector3.zero;

                transform.position = Vector3.Lerp(start, transform.position - margin * direction, 0.1f);
            }
    }
    void Awake()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "Kitty_001") catModel = child.gameObject;
            if (child.name == "Trail") dashModel = child.gameObject;
        }
        dashModel.SetActive(false);
        input = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        DashAction();
        
    }

    private void DashAction()
    { 
    if (!enableDash) return;

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer > 0f)
            {
                if (true)
                {
                    transform.position += (player.Velocity * Time.deltaTime);
                }
            }
            else
            {
                EndDash();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0f)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        player.Velocity = transform.forward * (dashDistance / dashDuration);

        catModel.SetActive(false);
        dashModel.SetActive(true);

        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = cooldownDuration;
    }

    private void EndDash()
    {
        catModel.SetActive(true);
        dashModel.SetActive(false);
        isDashing = false;
    }

}
