using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    private float currentHealth;
    private int maxHp;
    [Header("Invincible Settings")]
    [SerializeField] private float invincibleDuration = 0.03f; 
    private bool isInvincible = false;

    public float CurrentHealth => currentHealth;

    private void Start()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.Stat != null)
            currentHealth = PlayerManager.Instance.Stat.MaxHp;
        else if (PlayerStat.Instance != null)
            currentHealth = PlayerStat.Instance.MaxHp;
        else
            currentHealth = 100f;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= (int)damageInfo.Damage;

        float damage = damageInfo.Damage;
        DamagePopupManager.Show(transform.position, damage, isEnemyTarget: false, damageInfo.IsCritical);
        HUDController.Instance?.ShowDamageTaken(damage);

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

        float maxHealth = PlayerStat.Instance != null
            ? PlayerStat.Instance.MaxHp
            : currentHealth;

        if (PlayerManager.Instance != null && PlayerManager.Instance.Stat != null)
            maxHealth = PlayerManager.Instance.Stat.MaxHp;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
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
        Debug.Log("Player died.");

        if (DungeonManager.Instance != null)
            DungeonManager.Instance.ExitOrDie();
    }
}
