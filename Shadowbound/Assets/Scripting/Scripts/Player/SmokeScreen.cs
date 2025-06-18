using UnityEngine;
using UnityEngine.SceneManagement;

public class SmokeScreen : MonoBehaviour
{
    public static SmokeScreen Instance { get; private set; }
    private bool _hasCharge = true;
    public bool HasCharge
    {
        get => _hasCharge;
        set => _hasCharge = value;
    }
    
    [SerializeField] private float _yOffset = 0.135f; 
    void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
    }
        private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SmokeScreenAnimator.Instance.StopAnimation();
    }

    void Update()
    {
        if (!_hasCharge)
            return;

        if (!PlayerInput.Instance.ShadowScreen)
            return;
        transform.position = Player.Instance.transform.position + Vector3.up * _yOffset;
        _hasCharge = false;
        
        SmokeScreenAnimator.Instance.StartAnimation();
    }
}
