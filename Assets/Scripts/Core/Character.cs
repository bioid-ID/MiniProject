using UnityEngine;

public abstract class Character : MonoBehaviour
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
    A
    public virtual void SetStats(float hp, float atk, float physicaldef, float magicDef)
    {
        maxHp = hp;
        attackPower = atk;
        physicalDefense = physicaldef;
        magicDefense = magicDef;
        currentHp = maxHp;
    }
    public virtual void Heal(float amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
    }
}