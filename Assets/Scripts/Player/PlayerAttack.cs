using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Components")]
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

        float damage = stat.AttackDamage;
        EquipmentData weapon = stat.weaponSlot;

        if (weapon == null)
        {
            meleeAttack?.Attack(damage);
            return;
        }

        switch (weapon.AttackType)
        {
            case AttackType.Melee:
                meleeAttack?.Attack(damage);
                break;

            case AttackType.Projectile:
                projectileAttack?.Attack(damage);
                break;
        }
    }
}