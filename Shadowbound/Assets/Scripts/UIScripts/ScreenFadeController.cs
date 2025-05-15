using UnityEngine;

public class ScreenFadeController : MonoBehaviour
{
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
