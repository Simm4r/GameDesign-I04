
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconCooldown : MonoBehaviour
{
    [SerializeField] private Image _cooldownIcon;
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
                _cooldown = PossessionHandler.Instance.PossessionCooldown;
                _maxCooldown = PossessionHandler.Instance.PossessionMaxCooldown;
                break;

            case "ShadowVision_Icon":
                _cooldown = TacticalSight.Instance.ShadowVisionCooldown;
                _maxCooldown = TacticalSight.Instance.ShadowVisionMaxCooldown;
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
