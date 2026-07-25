using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;

    protected float currentCooldown;

    protected virtual void Update()
    {
        if (currentCooldown > 0)
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

    public abstract void Attack();
}