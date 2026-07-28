using UnityEngine;
using System.Collections;

public class MeleeAttack : AttackBase
{
    [Header("Melee Settings")]
    [SerializeField] private Hitbox meleeHitbox;
    [SerializeField] private float attackDuration = 0.2f;

    private Coroutine attackCoroutine;

    protected override void Awake()
    {

        base.Awake();

        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }

    public override void Attack(float finalDamage)
    {
        if (!CanAttack()) return;
        if (meleeHitbox == null) return;

        ResetCooldown();

        DamageInfo damageInfo = new DamageInfo(
            gameObject,
            finalDamage,
            DamageType.Physical,
            TeamType.Player
        );

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(AttackSequence(damageInfo));
    }

    private IEnumerator AttackSequence(DamageInfo damageInfo)
    {
        meleeHitbox.Initialize(damageInfo);
        meleeHitbox.gameObject.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        meleeHitbox.gameObject.SetActive(false);
        attackCoroutine = null;
    }

    private void OnDisable()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }
}
