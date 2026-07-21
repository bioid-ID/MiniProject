using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;

    private float damage;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    public void Launch(float dmg)
    {
        damage = dmg;
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            hurtbox.GetHit(damage);
            Destroy(gameObject);
        }
    }
}