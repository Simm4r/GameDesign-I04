using UnityEngine;

public class GodMode : MonoBehaviour
{
    public static GodMode Instance { get; private set; }

    public bool godMode = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void Update()
    {
        if (PlayerInput.Instance.GodMode)
        {
            godMode = !godMode;
        }
    }
}
