using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private GameObject owner;

    private IDamageable damageable;

    private void Awake()
    {
        if (owner == null)
            owner = gameObject;

        damageable = owner.GetComponent<IDamageable>();

        if (damageable == null)
            damageable = owner.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            Debug.LogError($"{name} : IDamageable을 찾지 못했습니다.");
        }
    }

    public void GetHit(DamageInfo damageInfo)
    {
        if (damageable != null)
            damageable.TakeDamage(damageInfo);
    }
}