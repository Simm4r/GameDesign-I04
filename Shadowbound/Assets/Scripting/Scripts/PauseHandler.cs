using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    public static PauseHandler Instance { get; private set; }
    private bool _inPause = false;
    public bool InPause
    {
        get => _inPause;
        set => _inPause = value;
    }
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
        if (PlayerInput.Instance.Pause)
        {
            _inPause = true;
            PauseMenu.Instance.Show();  
        }

    }
}
