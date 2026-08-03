using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Movement Settings")]
    [SerializeField] private bool useMouseRotation;
    [SerializeField] private bool dashEnabled = true;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private PlayerVisual playerVisual;
    private Camera mainCamera;
    private Vector2 moveInput;
    private float nextAttackTime;

    private readonly Collider2D[] detectionResults = new Collider2D[10];
    private ContactFilter2D contactFilter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerDash = GetComponent<PlayerDash>();
        playerVisual = GetComponent<PlayerVisual>();
        mainCamera = Camera.main;

        if (playerAttack == null)
        {
            playerAttack = GetComponent<PlayerAttack>();
            TryGetComponent(out playerAttack);
        }

        ApplyEnemyLayer();
    }

    private void Start()
    {
        ApplyEnemyLayer();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerDash == null)
            playerDash = GetComponent<PlayerDash>();

        if (playerVisual == null)
            playerVisual = GetComponent<PlayerVisual>();

        if (playerAttack == null)
            playerAttack = GetComponent<PlayerAttack>();
    }

    private void ApplyEnemyLayer()
    {
        if (enemyLayer.value == 0)
            enemyLayer = LayerMask.GetMask("Default");

        contactFilter.SetLayerMask(enemyLayer);
        contactFilter.useLayerMask = true;
    }

    private bool IsInHub => GameSceneNames.IsHubScene(SceneManager.GetActiveScene().name);

    private void Update()
    {
        if (GameStateController.Instance != null && !GameStateController.Instance.CanControlPlayer)
            return;

        ReadKeyboardFallback();

        if (useMouseRotation)
            RotateTowardsMouse();

        bool isDashing = playerDash != null && playerDash.IsDashing;
        playerVisual?.UpdateVisual(moveInput, isDashing);

        if (IsInHub)
            return;

        if (GameStateController.Instance != null && !GameStateController.Instance.CanCombat)
            return;

        HandleDash();
        HandleAutoAttack();
        HandleManualAttack();
        SkillManager.Instance?.UseAutoSkills();
    }

    private void HandleDash()
    {
        if (!dashEnabled || playerDash == null)
            return;

        if (Keyboard.current == null || !Keyboard.current.leftShiftKey.wasPressedThisFrame)
            return;

        Vector2 dashDirection = moveInput;
        if (dashDirection.sqrMagnitude < 0.01f && playerVisual != null)
            dashDirection = playerVisual.LastFacing;

        playerDash.TryDash(dashDirection);
    }

    private void HandleManualAttack()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TryAttack();
    }

    private void HandleAutoAttack()
    {
        if (playerAttack == null || PlayerStat.Instance == null)
            return;

        if (Time.time < nextAttackTime)
            return;

        float radius = Mathf.Max(PlayerStat.Instance.AttackRange, 2.5f);
        int targets = Physics2D.OverlapCircle(transform.position, radius, contactFilter, detectionResults);

        if (targets > 0)
            TryAttack(radius);
    }

    private void TryAttack(float radius = 2.5f)
    {
        if (playerAttack == null || PlayerStat.Instance == null)
            return;

        if (Time.time < nextAttackTime)
            return;

        float cooldown = 1.5f / PlayerStat.Instance.AttackSpeed;
        nextAttackTime = Time.time + cooldown;
        playerAttack.NormalAttack();
    }

    private void ReadKeyboardFallback()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
        }

        if (input.sqrMagnitude > 1f)
            input.Normalize();

        moveInput = input;
    }

    private void FixedUpdate()
    {
        if (playerDash != null && playerDash.IsDashing)
        {
            playerDash.ApplyDashVelocity();
            return;
        }

        Move();
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    private void Move()
    {
        float speed = PlayerStat.Instance != null ? PlayerStat.Instance.MoveSpeed : 5f;

        if (playerMovement != null)
            playerMovement.Move(moveInput, speed);
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 lookDir = (Vector2)mouseWorldPos - (Vector2)transform.position;

        if (lookDir.sqrMagnitude > 0.001f)
            rb.rotation = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
    }

    private void OnDrawGizmosSelected()
    {
        if (PlayerStat.Instance == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, PlayerStat.Instance.AttackRange);
    }
}
