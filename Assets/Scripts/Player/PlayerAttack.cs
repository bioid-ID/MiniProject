using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Prefab Settings")]
    [SerializeField] private GameObject projectilePrefab; 
    [SerializeField] private Transform firePoint;      

    private float attackCooldown = 0f;
    private bool isAttackPressed = false;

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (isAttackPressed && attackCooldown <= 0f)
        {
            Fire();
        }
    }
    public void OnAttack(InputValue value)
    {
        isAttackPressed = value.isPressed;
    }

    private void Fire()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("지정되지 않았습니다.");
            return;
        }
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        if (PlayerManager.Instance.Stat != null)
        {
            attackCooldown = 1f / PlayerManager.Instance.Stat.AttackSpeed;
        }
    }
}
