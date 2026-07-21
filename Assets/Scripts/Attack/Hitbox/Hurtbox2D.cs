using UnityEngine;

public class Hurtbox2D : MonoBehaviour
{
    [SerializeField] private GameObject owner;
    private IDamageable damageableTarget;

    private void Awake()
    {
        if (owner != null)
            damageableTarget = owner.GetComponent<IDamageable>();
    }

    public void GetHit(float damage)
    {
        if (damageableTarget != null)
            damageableTarget.TakeDamage(damage);
    }
}
