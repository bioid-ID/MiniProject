using UnityEngine;

public abstract class Character : PoolObject
{
    [Header("Character Stats")]
    [SerializeField] protected float maxHp = 100f;
    [SerializeField] protected float currentHp;
    [SerializeField] protected float attackPower = 10f;
    [SerializeField] protected float physicalDefense = 0f;
    [SerializeField] protected float magicDefense = 0f;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public float AttackPower => attackPower;
    public float PhysicalDefense => physicalDefense;
    public float MagicDefense => magicDefense;

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }
    public virtual void SetStats(float hp, float atk, float physicalDef, float magicDef)
    {
        maxHp = hp;
        attackPower = atk;
        physicalDefense = physicalDef;
        magicDefense = magicDef;
        currentHp = maxHp;
    }

    public virtual void Heal(float amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
    }

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        float calculatedDmg = damageInfo.Damage;
        if (damageInfo.IsCritical)
        {
            calculatedDmg *= damageInfo.CriticalMultiplier;
        }

        float effectiveDefense = physicalDefense * (1f - Mathf.Clamp01(damageInfo.IgnoreDefense));
        float finalDamage = Mathf.Max(1f, calculatedDmg - effectiveDefense);

        currentHp -= finalDamage;

        if (currentHp <= 0f)
        {
            Die();
        }
    }
    public void Kill()
    {
        Die();
    }
    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }
    public override void OnSpawn()
    {
        currentHp = maxHp;
    }
    public override void OnDespawn()
    {
    }
}
