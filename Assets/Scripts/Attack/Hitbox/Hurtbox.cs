using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [Header("??? ??? ????")]
    [SerializeField] private GameObject owner;
    [SerializeField] private TeamType team = TeamType.Neutral;

    private IDamageable damageable;

    private void Awake()
    {
        if (owner == null) owner = gameObject;

        if (!owner.TryGetComponent<IDamageable>(out damageable))
        {
            damageable = owner.GetComponentInParent<IDamageable>();
        }

        if (damageable == null)
        {
            Debug.LogError($"{name} : IDamageable?? ??? ????????.");
        }
    }

    public void GetHit(DamageInfo damageInfo)
    {
        if (damageable == null)
            return;

        if (damageInfo.Team == team)
            return;

        damageable.TakeDamage(damageInfo);
    }
}
