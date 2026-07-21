using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected LayerMask targetLayer;

    public abstract void ExecuteAttack();
}