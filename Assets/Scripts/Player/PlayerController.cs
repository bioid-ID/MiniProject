using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 moveInput;

    private void Awake()
    {
      rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        RotateTowardsMouse();
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

}

