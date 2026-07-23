using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    [Header("기본 공격 설정")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected LayerMask targetLayer;

    public abstract void ExecuteAttack();

    public virtual void ExecuteAttackWithModifier(float damageModifier)
    {
        ExecuteAttack();
    }
}
