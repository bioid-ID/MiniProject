using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private MeleeAttack meleeAttack;
    [SerializeField] private ProjectileAttack projectileAttack;

    private PlayerStat stat;

    private void Awake()
    {
        stat = PlayerStat.Instance;

        if (meleeAttack == null)
            meleeAttack = GetComponent<MeleeAttack>();

        if (projectileAttack == null)
            projectileAttack = GetComponent<ProjectileAttack>();
    }

    public void NormalAttack()
    {
        if (stat == null)
            return;

        switch (stat.CurrentAttackType)
        {
            case AttackType.Melee:
                meleeAttack.Attack(stat.AttackDamage);
                break;

            case AttackType.Projectile:
                projectileAttack.Attack(stat.AttackDamage);
                break;
        }
    }
}