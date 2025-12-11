using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDeath;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;
    public float HealthPercent => (float)currentHealth/maxHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    public void TakeDamage(int damage)
    {
        if(IsDead) return;
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }
}
