using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private MeleeAttack meleeAttack;
    [SerializeField] private ProjectileAttack projectileAttack;

    private PlayerStat stat;

    private void Awake()
    {
        ResolveComponents();
    }

    private void Start()
    {
        ResolveComponents();
    }

    public void NormalAttack()
    {
        ResolveComponents();

        if (stat == null || meleeAttack == null && projectileAttack == null)
            return;

        switch (stat.CurrentAttackType)
        {
            case AttackType.Melee:
                if (meleeAttack != null)
                    meleeAttack.Attack(stat.AttackDamage);
                break;

            case AttackType.Projectile:
                if (projectileAttack != null)
                    projectileAttack.Attack(stat.AttackDamage);
                break;
        }
    }

    private void ResolveComponents()
    {
        if (stat == null)
            stat = PlayerStat.Instance;

        if (meleeAttack == null)
            meleeAttack = GetComponent<MeleeAttack>();

        if (projectileAttack == null)
            projectileAttack = GetComponent<ProjectileAttack>();
    }
}
