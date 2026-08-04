using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    private Enemy enemy;
    private float currentHp;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void Initialize(float hp)
    {
        currentHp = hp;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        float damage = DamageCalculator.CalculateDamage(damageInfo, enemy);
        currentHp -= damage;

        DamagePopupManager.Show(transform.position, damage, isEnemyTarget: true, damageInfo.IsCritical);
        CombatHitUtility.ApplyOnHitEffects(enemy, damageInfo, damage);

        if (currentHp <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        currentHp = Mathf.Min(currentHp + amount, enemy.MaxHp);
    }

    private void Die()
    {
        GameFeel.EnemyKilled();
        EnemySpawnerManager.Instance?.OnMonsterKilled(enemy, transform.position);
        enemy.Kill();
    }
}
