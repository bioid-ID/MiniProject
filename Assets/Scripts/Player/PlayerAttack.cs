using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private ProjectileAttack2D projectileAttack;
    [SerializeField] private MeleeAttack2D meleeAttack;

    private PlayerStat stat;

    private void Awake()
    {
        stat = GetComponent<PlayerStat>();

        if (projectileAttack == null)
            projectileAttack = GetComponent<ProjectileAttack2D>();

        if (meleeAttack == null)
            meleeAttack = GetComponent<MeleeAttack2D>();
    }

    public void NormalAttack()
    {
        if (stat == null) return;

        if (stat.AttackType == AttackType.Melee)
        {
            meleeAttack.Attack();
        }
        else
        {
            projectileAttack.Attack();
        }
    }
}