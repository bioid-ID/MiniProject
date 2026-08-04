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
        ResolveHitbox();
    }

    private void Start()
    {
        ResolveHitbox();
        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }

    private void ResolveHitbox()
    {
        if (meleeHitbox != null)
            return;

        Transform hitboxTransform = transform.Find("MeleeHitbox");
        if (hitboxTransform != null)
            meleeHitbox = hitboxTransform.GetComponent<Hitbox>();
    }

    public override void Attack(float finalDamage)
    {
        if (!CanAttack())
            return;

        ResolveHitbox();

        if (meleeHitbox == null)
            return;

        ResetCooldown();

        Vector2 fallback = Vector2.right;
        PlayerVisual visual = GetComponent<PlayerVisual>();
        if (visual != null)
            fallback = visual.LastFacing;

        Vector2 aimDirection = PlayerAim.GetAttackDirection(transform, fallback);
        DamageInfo damageInfo = CombatHitUtility.BuildPlayerAttack(
            gameObject,
            finalDamage,
            aimDirection,
            AttackMethod.Melee);

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(AttackSequence(damageInfo, aimDirection));
    }

    private IEnumerator AttackSequence(DamageInfo damageInfo, Vector2 aimDirection)
    {
        PlayerAim.ApplyDirection(meleeHitbox.transform, aimDirection);
        meleeHitbox.transform.localPosition = new Vector3(aimDirection.x, aimDirection.y, 0f) * 0.6f;

        meleeHitbox.Initialize(damageInfo);
        meleeHitbox.gameObject.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        meleeHitbox.gameObject.SetActive(false);
        meleeHitbox.transform.localPosition = Vector3.zero;
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
