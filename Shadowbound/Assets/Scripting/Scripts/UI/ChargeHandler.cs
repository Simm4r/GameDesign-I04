using UnityEngine;
using UnityEngine.UI;

public class ChargeHandler : MonoBehaviour
{
    public static ChargeHandler Instance { get; private set; }
    [SerializeField] private Image _cooldownIcon;
    private bool _hasCharge;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        _hasCharge = SmokeScreen.Instance.HasCharge;    
    }

    void Update()
    {
        if (_hasCharge != SmokeScreen.Instance.HasCharge)
        {
            _hasCharge = SmokeScreen.Instance.HasCharge;
            float fillAmount = _hasCharge ? 0.0f : 1.0f;
            _cooldownIcon.fillAmount = fillAmount;
        }
    }
}
