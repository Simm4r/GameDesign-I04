using System;
using UnityEngine;


public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    [SerializeField] private float _baseDamageTaken = 5f;
    [SerializeField] private float _damageOverTimeInterval = 0.5f;
    private float _tickDamageTimer = 0f;

    [SerializeField] private float _baseDamageHealed = 5f;
    [SerializeField] private float _healOverTimeInterval = 0.5f;
    private float _healTimer = 0f;

    [SerializeField] private int _possessionLevel = 1;
    [SerializeField] private int _shadowVisionLevel = 1;
    [SerializeField] private int _shadowScreenLevel = 1;

    [SerializeField] private bool _isDead = false;


    public event Action OnPlayerDeath;

    public int PossessionLevel
    {
        get => _possessionLevel;
        set => _possessionLevel = value;
    }

    public int ShadowVisionLevel
    {
        get => _shadowVisionLevel;
        set => _shadowVisionLevel = value;
    }

    public int ShadowScreenLevel
    {
        get => _shadowScreenLevel;
        set => _shadowScreenLevel = value;
    }
    public float MaxHealth => _maxHealth;

    public float CurrentHealth => _currentHealth;

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
    } 
    private void Start()
    {
        _currentHealth = _maxHealth;
        Healthbar.Instance.UpdateHealthbar(_maxHealth, _currentHealth);
    }

    public void TakeDamage()
    {

        if (_currentHealth <= 0)
        {
            Die();
            return;
        }

        if (_tickDamageTimer > 0f)
        {
            _tickDamageTimer -= Time.deltaTime;
            return;
        }

        _tickDamageTimer = _damageOverTimeInterval;
        _currentHealth -= _baseDamageTaken;
        Healthbar.Instance.UpdateHealthbar(_maxHealth, _currentHealth);

    }

    public void HealDamage()
    {
        if (_currentHealth >= _maxHealth)
        {
            return;
        }

        if (_healTimer > 0f)
        {
            _healTimer -= Time.deltaTime;
            return;
        }

        _healTimer = _healOverTimeInterval;
        _currentHealth += _baseDamageHealed;
        Healthbar.Instance.UpdateHealthbar(_maxHealth, _currentHealth);

    }

    public void ResetTimers()
    {
        _tickDamageTimer = 0f;
        _healTimer = 0f;
    }

    private void Die()
    {
        if (_isDead) return;

        _isDead = true;


        OnPlayerDeath?.Invoke(); // Notifica esterna
    }

    public void ResetPlayer()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
        Healthbar.Instance.ResetHealthbar();
    }
}
