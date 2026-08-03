using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 14f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 1.2f;

    private Rigidbody2D rb;
    private float dashEndTime;
    private float cooldownEndTime;
    private Vector2 dashDirection;

    public bool IsDashing => Time.time < dashEndTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public bool TryDash(Vector2 direction)
    {
        if (IsDashing || Time.time < cooldownEndTime)
            return false;

        if (direction.sqrMagnitude < 0.01f)
            direction = transform.right;

        dashDirection = direction.normalized;
        dashEndTime = Time.time + dashDuration;
        cooldownEndTime = Time.time + dashCooldown;
        return true;
    }

    public void ApplyDashVelocity()
    {
        if (!IsDashing)
            return;

        rb.linearVelocity = dashDirection * dashSpeed;
    }
}
