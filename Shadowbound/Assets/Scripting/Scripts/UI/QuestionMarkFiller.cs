using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestionMarkFiller : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _maxTime = 1.5f;

    public void SetFill(float stateTimer)
    {
        float t = Mathf.Clamp01(stateTimer / _maxTime);
        _fillImage.fillAmount = t;
    }

    public void ResetFill()
    {
        _fillImage.fillAmount = 0f;
    }

    public void SetMaxFill()
    {
        _fillImage.fillAmount = 1f;
    }
}
