
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconCooldown : MonoBehaviour
{
    [SerializeField] private Image _cooldownIcon;
    [SerializeField] private PossessionHandler _possessionHandler;
    [SerializeField] private TacticalSight _tacticalSight;
    [SerializeField] private TextMeshProUGUI _timer;
    private float _maxCooldown;
    private float _cooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    }

    private void GetActualCooldown()
    {
        switch (tag)
        {
            case "Possession_Icon":
                _cooldown = _possessionHandler.PossessionCooldown;
                _maxCooldown = _possessionHandler.PossessionMaxCooldown;
                break;

            case "ShadowVision_Icon":
                _cooldown = _tacticalSight.ShadowVisionCooldown;
                _maxCooldown = _tacticalSight.ShadowVisionMaxCooldown;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        GetActualCooldown();
        _cooldownIcon.fillAmount = _cooldown / _maxCooldown;
        int secondsLeft = Mathf.CeilToInt(_cooldown);
        _timer.text = secondsLeft > 0 ? secondsLeft.ToString() : "";
    }
}
