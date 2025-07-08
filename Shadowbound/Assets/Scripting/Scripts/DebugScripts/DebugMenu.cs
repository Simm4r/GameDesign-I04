using KinematicCharacterController;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button freezeGuardsButton;
    public Button panicKillButton;
    public Button invincibleButton;
    public Button teleportButton;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject teleportRoomsPanel;

    [Header("Teleport Room Buttons")]
    public Button ratsDenButton;
    public Button keyRoomButton;
    public Button lockedRoomButton;
    public Button backButton;

    [Header("Teleport Targets")]
    public Transform ratsDenTarget;
    public Transform keyRoomTarget;
    public Transform lockedRoomTarget;

    private bool guardsFrozen = false;
    private bool isInvincible = false;

    private Image freezeGuardsButtonImage;
    private Image invincibleButtonImage;
    void Start()
    {
        invincibleButtonImage = invincibleButton.GetComponent<Image>();
        freezeGuardsButtonImage = freezeGuardsButton.GetComponent<Image>();

        mainPanel.SetActive(false);
        teleportRoomsPanel.SetActive(false);

        freezeGuardsButton.onClick.AddListener(() => { Debug.Log("Freeze Guards CLICKED"); ToggleFreezeGuards(); });
        panicKillButton.onClick.AddListener(() => { Debug.Log("Panic Kill CLICKED"); PanicKillMomo(); });
        invincibleButton.onClick.AddListener(() => { Debug.Log("Toggle Invincible CLICKED"); ToggleInvincible(); });
        teleportButton.onClick.AddListener(() => { Debug.Log("Teleport CLICKED"); ShowTeleportRoomsPanel(); });

        ratsDenButton.onClick.AddListener(() => TeleportToRoom(ratsDenTarget));
        keyRoomButton.onClick.AddListener(() => TeleportToRoom(keyRoomTarget));
        lockedRoomButton.onClick.AddListener(() => TeleportToRoom(lockedRoomTarget));
        backButton.onClick.AddListener(() => ShowMainPanel());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (!mainPanel.activeSelf)
            {
                mainPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PauseHandler.Instance.InPause = true;
                PauseMenu.Instance.gameObject.SetActive(false);
            }
            else
            {
                HideAllPanels();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PauseHandler.Instance.InPause = false;
                PauseMenu.Instance.gameObject.SetActive(true);
            }
        }

        if (mainPanel.activeSelf || teleportRoomsPanel.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HideAllPanels()
    {
        mainPanel.SetActive(false);
        teleportRoomsPanel.SetActive(false);
    }

    void ShowTeleportRoomsPanel()
    {
        mainPanel.SetActive(false);
        teleportRoomsPanel.SetActive(true);
    }

    void ShowMainPanel()
    {
        teleportRoomsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ToggleFreezeGuards()
    {
        guardsFrozen = !guardsFrozen;
        Debug.Log($"Freeze Guards: {guardsFrozen}");
        var guards = FindObjectsByType<GuardPatrol>(FindObjectsSortMode.None);
        foreach (var guard in guards)
        {
            if (guardsFrozen)
                guard.FreezeGuard();
            else
                guard.UnfreezeGuard();
        }
        if(freezeGuardsButtonImage != null)
        {
            freezeGuardsButtonImage.color = guardsFrozen ? Color.green : Color.white;
        }
    }

    public void PanicKillMomo()
    {
        PlayerStats.Instance.Die();
        Debug.Log("Panic Kill triggered");
    }

    public void ToggleInvincible()
    {
        isInvincible = !isInvincible;
        PlayerStats.Instance.CanBeHit = !isInvincible;
        Debug.Log($"Momo Invincible: {isInvincible}");

        if (invincibleButtonImage != null)
        {
            invincibleButtonImage.color = isInvincible ? Color.green : Color.white;
        }
    }

    public void TeleportToRoom(Transform target)
    {
        if (target != null)
        {
            Player.Instance.GetComponent<KinematicCharacterMotor>().SetPosition(target.position);
            Debug.Log($"Teleported to {target.name} at {target.position}");
        }
        else
        {
            Debug.LogWarning("Teleport target is not assigned in the Inspector.");
        }
    }
}
