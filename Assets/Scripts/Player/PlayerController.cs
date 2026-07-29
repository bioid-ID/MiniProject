using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement; 
    private Camera mainCamera;
    private Vector2 moveInput;
    private float nextAttackTime;

    private readonly Collider2D[] detectionResults = new Collider2D[10];
    private ContactFilter2D contactFilter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        mainCamera = Camera.main;

        if (playerAttack == null)
        {
            playerAttack = GetComponent<PlayerAttack>();
            TryGetComponent(out playerAttack);

        }

        contactFilter.SetLayerMask(enemyLayer);
        contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        RotateTowardsMouse();
        HandleAutoAttack();
        SkillManager.Instance.UseAutoSkills();
    }

    private void FixedUpdate() => Move();

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    private void Move()
    {
        float speed = PlayerStat.Instance != null ? PlayerStat.Instance.MoveSpeed : 5f;

        if (playerMovement != null)
        {
            playerMovement.Move(moveInput, speed);
        }
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null) return;

        if (Mouse.current == null) return;
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

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

        int targets = Physics2D.OverlapCircle(transform.position, radius, contactFilter, detectionResults);

        if (targets > 0)
        {
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
