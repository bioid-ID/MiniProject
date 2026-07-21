using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile2D : MonoBehaviour
{
    [Header("≈ıªÁ√º")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 2.5f;

    private float damage;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); 
    }

    public void Launch(float dmg)
    {
        damage = dmg;

        rb2d.linearVelocity = transform.right * speed; 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Hurtbox2D hurtbox = other.GetComponent<Hurtbox2D>();
        if (hurtbox != null)
        {
            hurtbox.GetHit(damage);
            Destroy(gameObject); 
        }
    }
}
