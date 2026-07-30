using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : PoolObject
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 2f;

    private Rigidbody2D rb;

    private float timer;

    private DamageInfo damageInfo;

    private int piercing;
    private float decay;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnSpawn()
    {
        timer = lifeTime;
    }

    public override void OnDespawn()
    {
        rb.linearVelocity = Vector2.zero;

        damageInfo = default;
        piercing = 0;
        decay = 0f;
        timer = 0f;
    }

    public void Launch(
        float damage,
        int piercing,
        float decay,
        DamageType damageType,
        TeamType team)
    {
        this.piercing = piercing;
        this.decay = decay;

        damageInfo = new DamageInfo(
            gameObject,
            damage,
            damageType,
            team);

        rb.linearVelocity = transform.right * speed;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PoolManager.Instance.Return(PoolKey.Projectile, this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();

        if (hurtbox == null)
            return;

        hurtbox.GetHit(damageInfo);

        if (piercing > 0)
        {
            piercing--;

            damageInfo.Damage *= (1f - decay);
        }
        else
        {
            PoolManager.Instance.Return(PoolKey.Projectile, this);
        }
    }
}