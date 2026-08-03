using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private float moveSpeed;

    private Vector2 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float speed)
    {
        moveSpeed = speed;
    }

    public void Move(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    public void Stop()
    {
        moveDirection = Vector2.zero;
    }

    [SerializeField] private float separationRadius = 0.3f;
    [SerializeField] private float separationForce = 1f;

    private void FixedUpdate()
    {
        Vector2 velocity = moveDirection * moveSpeed + GetSeparation();
        rb.linearVelocity = velocity;
    }

    private Vector2 GetSeparation()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        Vector2 push = Vector2.zero;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.GetComponent<Enemy>() == null)
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
        rb.linearVelocity = Vector2.zero;
        moveDirection = Vector2.zero;
    }
}