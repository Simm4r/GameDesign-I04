using UnityEngine;

public class ScreenFadeController : MonoBehaviour
{
    public static ScreenFadeController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
    }
    [SerializeField] private Animator _animator;

    public void FadeToBlack()
    {
        _animator.Play("FadeIn");
    }

    public void FadeFromBlack()
    {
        _animator.Play("FadeOut");
    }
}
