using UnityEngine;

public class MeleeAttack2D : AttackBase
{
    [Header("2D 근접 공격 설정")]
    [SerializeField] private Hitbox2D meleeHitbox;

    private void Awake()
    {
        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }

    public override void ExecuteAttack()
    {
        if (meleeHitbox != null)
        {
            meleeHitbox.Initialize(damage);
            meleeHitbox.gameObject.SetActive(true);
            Invoke(nameof(DisableHitbox), 0.2f);
        }
    }

    private void DisableHitbox()
    {
        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }
}
