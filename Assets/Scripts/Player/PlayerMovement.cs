using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody2D 기본 설정 보장
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }


    public void Move(Vector2 moveInput, float speed)
    {
        // 대각선 이동 속도 보정
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        // 물리 속도(Velocity) 직접 제어
        rb.linearVelocity = moveInput * speed;
    }

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
