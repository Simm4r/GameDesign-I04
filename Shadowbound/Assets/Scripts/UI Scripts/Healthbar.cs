using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Transform _follow; // il transform del player
    [SerializeField] private Vector3 _offset = new Vector3(0.3f, 0.5f, 0);

    [SerializeField] private Image _healthbarSprite;
    [SerializeField] private float _reduceSpeed = 2f;
    private float _target = 1;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeSpeed = 2f;
    [SerializeField] private float _visibleDuration = 2f;
    private float _visibleTimer = 0f;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main; 
    }

    public void UpdateHealthbar(float maxHealth, float currentHealth)
    {
        _target = currentHealth / maxHealth;

        if (_target < 1f)
        {
            _visibleTimer = _visibleDuration;
        }
        else if (_canvasGroup.alpha > 0f)
        {
            _visibleTimer = _visibleDuration;
        }
    }

    public void ResetHealthbar()
    {
        _target = 1f;
        _healthbarSprite.fillAmount = _target;
    }
    private void Update()
    {
        _healthbarSprite.fillAmount = Mathf.MoveTowards(_healthbarSprite.fillAmount, _target, _reduceSpeed * Time.deltaTime);

        if (_target >= 1f)
        {
            _visibleTimer -= Time.deltaTime;
        }

        float targetAlpha = (_target < 1f || _visibleTimer > 0f) ? 1f : 0f;
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, _fadeSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        transform.position = _follow.position + _offset;
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
    }
}
