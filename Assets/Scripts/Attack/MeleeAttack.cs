using UnityEngine;

public class MeleeAttack : AttackBase
{
    [Header("Melee Settings")]
    [SerializeField] private Hitbox meleeHitbox;

    private void Awake()
    {
        if(meleeHitbox != null) 
            meleeHitbox.gameObject.SetActive(false);
    }

    public override void ExecuteAttack()
    {
        if (meleeHitbox != null)
        {
            meleeHitbox.Initialize(damage);
            meleeHitbox.gameObject.SetActive(true);
            Invoke(nameof(DisableHitbox), 0.3f);
        }
    }

    private void DisableHitbox()
    {
        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }
}