using System.Collections;
using UnityEngine;

public class ScreenFadeController : MonoBehaviour
{
    public static ScreenFadeController Instance { get; private set; }

    [SerializeField] private Animator _animator;
    public Animator Animator
    {
        get => _animator;
    }
    private bool _isFading = false;

    public bool IsFading
    {
        get => _isFading;
     }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        StartCoroutine(SetUnscaledDeltaTime());
    }

    IEnumerator SetUnscaledDeltaTime()
    {
        yield return new WaitForSeconds(1.5f);
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    } 

    private string currentStateName = "";

    public void FadeToBlack()
    {
        _animator.Play("FadeIn");
        _isFading = true;
        currentStateName = "FadeIn";
    }

    public void FadeFromBlack()
    {
        _animator.Play("FadeOut");
        _isFading = true;
        currentStateName = "FadeOut";
    }

    private void Update()
    {
        if (!_isFading) return;

        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Idle") && currentStateName == "FadeOut")
        {
            _isFading = false;
            currentStateName = "";
        }
    }
}
