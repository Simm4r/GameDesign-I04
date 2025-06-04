using UnityEngine;

public class DitherHandler : MonoBehaviour
{
    private float _ditherAmount;
    [SerializeField] private Renderer _rend;
    [SerializeField] private Outline _outline;

    void Update()
    {
        if (PlayerInput.Instance.InPossession)
            return;

        if (_rend.sharedMaterial.HasProperty("_Alpha"))
        {
            Color outlineColor;
            if (PlayerInput.Instance.Dying)
            {
                _rend.sharedMaterial.SetFloat("_Alpha", 0);
                outlineColor = _outline.OutlineColor;
                outlineColor.a = 0.0f;
                _outline.OutlineColor = outlineColor;
                return;
            }
            _ditherAmount = PlayerStats.Instance.CurrentHealth / PlayerStats.Instance.MaxHealth;
            _rend.sharedMaterial.SetFloat("_Alpha", _ditherAmount);
            outlineColor = _outline.OutlineColor;
            outlineColor.a = _ditherAmount;
            _outline.OutlineColor = outlineColor;
        }
        
    }
}
