using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    private float currentHealth;
    private int maxHp;
    [Header("Invincible Settings")]
    [SerializeField] private float invincibleDuration = 0.5f; 
    private bool isInvincible = false;

    public float CurrentHealth => currentHealth;

    private void Start()
    {
        if (PlayerManager.Instance.Stat != null)
        {
            currentHealth = PlayerManager.Instance.Stat.MaxHp;
        }
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= (int)damageInfo.Damage;
        Debug.Log($"Ã¼·Â: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibleRoutine());
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        float maxHp = PlayerManager.Instance.Stat.MaxHp;

        if (currentHealth > maxHp) currentHealth = maxHp;
    }

    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void Die()
    {
        currentHealth = 0;
        Debug.LogError("»ç¸Á");
    }
}
