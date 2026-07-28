using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [Header("피격 주체 설정")]
    [SerializeField] private GameObject owner;

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
            Debug.LogError($"{name} : IDamageable을 찾지 못했습니다.");
        }
    }

    public void GetHit(DamageInfo damageInfo)
    {
        if (damageable != null)
        {
            damageable.TakeDamage(damageInfo);
        }
    }
}
