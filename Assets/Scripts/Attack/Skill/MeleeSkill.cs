using UnityEngine;

public class MeleeSkill : SkillBase
{
    [SerializeField]
    private Hitbox hitbox;

    protected override void Use()
    {
        DamageInfo info =
            new DamageInfo(
                gameObject,
                data.damage,
                DamageType.Physical,
                TeamType.Player);

        hitbox.Initialize(info);

        hitbox.gameObject.SetActive(true);

        Invoke(nameof(DisableHitbox), 0.15f);
    }

    private void DisableHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }
}