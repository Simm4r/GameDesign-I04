using System;
using UnityEngine;
using UnityEngine.InputSystem;


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
    [SerializeField] private AudioSource _deathSound;

    [SerializeField] private bool _isDead = false;
    private bool _hitByLaser = false;
    public bool HitByLaser
    {
        get => _hitByLaser;
        set => _hitByLaser = value;
    }

    public bool CanBeHit = true;

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

    public float CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    } 

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

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
            StopRumble();
            Die();
            return;
        }

        if (_tickDamageTimer > 0f)
        {
            _tickDamageTimer -= Time.deltaTime;
            return;
        }
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0.1f, 0.5f);
        }
        _tickDamageTimer = _damageOverTimeInterval;
        _currentHealth -= _baseDamageTaken;
        Healthbar.Instance.UpdateHealthbar(_maxHealth, _currentHealth);

    }

    void StopRumble()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
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
        StopRumble();
        _healTimer = _healOverTimeInterval;
        _currentHealth += _baseDamageHealed;
        Healthbar.Instance.UpdateHealthbar(_maxHealth, _currentHealth);

    }

    public void ResetTimers()
    {
        _tickDamageTimer = 0f;
        _healTimer = 0f;
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _deathSound.Stop();
        _deathSound.Play();
        OnPlayerDeath?.Invoke(); // Notifica esterna
    }

    public void ResetPlayer()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
        Healthbar.Instance.ResetHealthbar();
    }
}
