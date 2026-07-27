using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 moveInput;
    private float nextAttackTime;

    private readonly Collider2D[] detectionResults = new Collider2D[10];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (playerAttack == null) playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        RotateTowardsMouse();
        HandleAutoAttack();
    }

    private void FixedUpdate() => Move();

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    private void Move()
    {
        if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
        float speed = PlayerStat.Instance != null ? PlayerStat.Instance.MoveSpeed : 5f;
        rb.linearVelocity = moveInput * speed;
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null) return;
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 lookDir = (Vector2)mouseWorldPos - (Vector2)transform.position;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            rb.rotation = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        }
    }

    private void HandleAutoAttack()
    {
        if (playerAttack == null || PlayerStat.Instance == null) return;
        if (Time.time < nextAttackTime) return;

        float radius = PlayerStat.Instance.AttackRange;
        int targets = Physics2D.OverlapCircleNonAlloc(transform.position, radius, detectionResults, enemyLayer);

        if (targets > 0)
        {
            // 공속에 반비례하는 쿨타임 연산
            float cooldown = 1.5f / PlayerStat.Instance.AttackSpeed;
            nextAttackTime = Time.time + cooldown;

            playerAttack.NormalAttack();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (PlayerStat.Instance == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, PlayerStat.Instance.AttackRange);
    }
}
