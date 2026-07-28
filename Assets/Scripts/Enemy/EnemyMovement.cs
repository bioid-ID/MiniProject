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

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    private void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;
        moveDirection = Vector2.zero;
    }
}