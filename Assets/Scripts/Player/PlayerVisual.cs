using UnityEngine;

[DisallowMultipleComponent]
public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool flipByMovement = true;

    private Animator animator;
    private Vector2 lastFacing = Vector2.down;

    public Vector2 LastFacing => lastFacing;

    private void Awake()
    {
        if (spriteRenderer == null)
            TryGetComponent(out spriteRenderer);

        animator = GetComponent<Animator>();
    }

    public void UpdateVisual(Vector2 moveInput, bool isDashing)
    {
        if (moveInput.sqrMagnitude > 0.01f)
            lastFacing = moveInput.normalized;

        if (HasActiveAnimator())
        {
            animator.SetFloat("MoveX", lastFacing.x);
            animator.SetFloat("MoveY", lastFacing.y);
            animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0.01f);
            animator.SetBool("IsDashing", isDashing);
            return;
        }

        if (!flipByMovement || spriteRenderer == null)
            return;

        if (Mathf.Abs(lastFacing.x) > 0.01f)
            spriteRenderer.flipX = lastFacing.x < 0f;
    }

    private bool HasActiveAnimator()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }
}
