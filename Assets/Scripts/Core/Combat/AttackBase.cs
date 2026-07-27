using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    [SerializeField] protected float cooldown = 1f;

    protected float currentCooldown;

    protected virtual void Awake()
    {
    }

    protected virtual void Update()
    {
        if (currentCooldown > 0f)
            currentCooldown -= Time.deltaTime;
    }

    public bool CanAttack()
    {
        return currentCooldown <= 0f;
    }

    protected void ResetCooldown()
    {
        currentCooldown = cooldown;
    }

    public abstract void Attack(float finalDamage);
}