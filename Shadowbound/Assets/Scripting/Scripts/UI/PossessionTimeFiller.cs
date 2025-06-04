using UnityEngine;
using UnityEngine.UI;

public class PossessionTimeFiller : MonoBehaviour
{
    private Image _bar;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _bar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        _bar.fillAmount = 1 - (PossessionHandler.Instance.PossessionTime / PossessionHandler.Instance.PossessionMaxTime);
    }
}
