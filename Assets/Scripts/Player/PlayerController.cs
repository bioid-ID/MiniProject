using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Combat Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private AttackBase currentAttack;

    [Header("Auto Attack Settings")]
    [SerializeField] private float baseAttackCooldown = 1.5f;
    [SerializeField] private float baseDetectionRadius = 8f;
    [SerializeField] private LayerMask enemyLayer;

    private float currentHealth;
    private float nextAttackTime;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 moveInput;

    private float damageModifier = 1f;
    private float cooldownModifier = 1f;
    private float radiusModifier = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        currentHealth = maxHealth;
    }

    private void Update()
    {
        RotateTowardsMouse();
        HandleAutoAttack();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Move()
    {
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null) return;
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = (Vector2)mousePosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    private void HandleAutoAttack()
    {
        if (currentAttack == null) return;
        if (Time.time < nextAttackTime) return;

        float currentRadius = baseDetectionRadius * radiusModifier;
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, currentRadius, enemyLayer);

        if (targets.Length > 0)
        {
            float currentCooldown = baseAttackCooldown * cooldownModifier;
            nextAttackTime = Time.time + currentCooldown;

            currentAttack.ExecuteAttackWithModifier(damageModifier);
        }
    }

    public void ApplyStatModifiers(float damageMult, float cooldownMult, float radiusMult)
    {
        damageModifier *= damageMult;
        cooldownModifier *= cooldownMult;
        radiusModifier *= radiusMult;
    }

    public void ResetStatModifiers()
    {
        damageModifier = 1f;
        cooldownModifier = 1f;
        radiusModifier = 1f;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, baseDetectionRadius * radiusModifier);
    }
}
