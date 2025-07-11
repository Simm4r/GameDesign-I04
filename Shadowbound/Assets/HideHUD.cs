using UnityEngine;

public class HideHUD : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    private bool _hide = false;
    void Update()
    {
        if (PlayerInput.Instance.HideHud)
        {
            _hide = !_hide;
            _group.alpha = _hide ? 0 : 1; 
        }
    }
}

