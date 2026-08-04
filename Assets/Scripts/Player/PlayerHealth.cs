using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    private float currentHealth;

    [Header("Invincible Settings")]
    [SerializeField] private float invincibleDuration = 0.03f;

    private bool isInvincible;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        ReviveFull();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (isInvincible || isDead || currentHealth <= 0f)
            return;

        currentHealth -= damageInfo.Damage;

        float damage = damageInfo.Damage;
        DamagePopupManager.Show(transform.position, damage, isEnemyTarget: false, damageInfo.IsCritical);
        HUDController.Instance?.ShowDamageTaken(damage);
        GameFeel.PlayerHit(damage);
        DungeonManager.Instance?.RunStats.LogDamageTaken(damage);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibleRoutine());
    }

    public void Heal(float amount, HealSource source = HealSource.Other)
    {
        if (isDead || amount <= 0f)
            return;

        float maxHealth = GetMaxHealth();
        float before = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        float actual = currentHealth - before;

        if (actual > 0f)
            DungeonManager.Instance?.RunStats.LogHeal(actual, source);
    }

    public void ReviveFull()
    {
        isDead = false;
        isInvincible = false;
        currentHealth = GetMaxHealth();
        PlayerStat.Instance?.RestoreMp(
            PlayerStat.Instance != null ? PlayerStat.Instance.MaxMp : 0f,
            ManaSource.Other);
    }

    private float GetMaxHealth()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.Stat != null)
            return PlayerManager.Instance.Stat.MaxHp;

        if (PlayerStat.Instance != null)
            return PlayerStat.Instance.MaxHp;

        return 100f;
    }

    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0f;
        isInvincible = true;
        Debug.Log("Player died — showing run result.");

        if (DungeonManager.Instance != null)
        {
            DungeonManager.Instance.ExitOrDie();
            return;
        }

        Debug.LogError("Player died but DungeonManager.Instance is null.");
    }
}
