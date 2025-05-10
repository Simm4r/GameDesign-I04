using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    [SerializeField] private float _baseDamageTaken = 5f;
    [SerializeField] private float _damageOverTimeInterval = 0.5f;
    private float _tickDamageTimer = 0f;

    [SerializeField] private float _baseDamageHealed = 5f;
    [SerializeField] private float _healOverTimeInterval = 0.5f;
    private float _healTimer = 0f;


    [SerializeField] private bool _isDead = false;

    [SerializeField] private Healthbar _healthbar;

    public event Action OnPlayerDeath;

    private void Start()
    {
        _currentHealth = _maxHealth;
        _healthbar.UpdateHealthbar(_maxHealth, _currentHealth);
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
        _healthbar.UpdateHealthbar(_maxHealth, _currentHealth);

        Debug.Log($"Current health: {_currentHealth}");
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
        _healthbar.UpdateHealthbar(_maxHealth, _currentHealth);

        Debug.Log($"Current health: {_currentHealth}");
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
        Debug.Log("Player morto");

        OnPlayerDeath?.Invoke(); // Notifica esterna
    }

    public void ResetPlayer()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
        _healthbar.ResetHealthbar();
    }
}
