using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float separationRadius = 0.35f;
    [SerializeField] private float separationForce = 1.2f;
    [SerializeField] private float knockbackDrag = 8f;

    private Rigidbody2D rb;
    private float moveSpeed;
    private Vector2 moveDirection;
    private Vector2 knockbackVelocity;
    private float stunTimer;

    public bool IsStunned => stunTimer > 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        knockbackVelocity = Vector2.zero;
        stunTimer = 0f;
        moveDirection = Vector2.zero;

        if (rb != null)
            rb.WakeUp();
    }

    public void Initialize(float speed)
    {
        moveSpeed = speed;
    }

    public void Move(Vector2 direction)
    {
        if (IsStunned)
            return;

        moveDirection = direction.normalized;
    }

    public void Stop()
    {
        moveDirection = Vector2.zero;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (force <= 0f)
            return;

        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.up;

        knockbackVelocity += direction.normalized * force;
    }

    public void ApplyStun(float duration)
    {
        if (duration <= 0f)
            return;

        stunTimer = Mathf.Max(stunTimer, duration);
        moveDirection = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (stunTimer > 0f)
            stunTimer -= Time.fixedDeltaTime;

        Vector2 velocity = knockbackVelocity;

        if (!IsStunned)
            velocity += moveDirection * moveSpeed + GetSeparation();

        rb.linearVelocity = velocity;
        knockbackVelocity = Vector2.MoveTowards(
            knockbackVelocity,
            Vector2.zero,
            knockbackDrag * Time.fixedDeltaTime);
    }

    private Vector2 GetSeparation()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        Vector2 push = Vector2.zero;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.GetComponentInParent<Enemy>() == null)
                continue;

            Vector2 away = (Vector2)transform.position - (Vector2)hit.transform.position;
            if (away.sqrMagnitude < 0.001f)
                continue;

            push += away.normalized / away.magnitude;
        }

        return push * separationForce;
    }

    private void OnDisable()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        moveDirection = Vector2.zero;
        knockbackVelocity = Vector2.zero;
        stunTimer = 0f;
    }
}
