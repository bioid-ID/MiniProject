using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerSetupUtility
{
    public static void Apply(GameObject playerObject, PlayerSetupMode mode)
    {
        if (playerObject == null)
            return;

        playerObject.tag = "Player";
        ApplyPhysics(playerObject);
        DisablePlayerInput(playerObject);
        ApplySharedComponents(playerObject);

        if (mode == PlayerSetupMode.Hub)
            ApplyHubComponents(playerObject);
        else
            ApplyDungeonComponents(playerObject);

        ApplyProgress(playerObject);
    }

    private static void ApplyPhysics(GameObject playerObject)
    {
        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>() ?? playerObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        CircleCollider2D bodyCollider = playerObject.GetComponent<CircleCollider2D>();
        if (bodyCollider == null)
        {
            bodyCollider = playerObject.AddComponent<CircleCollider2D>();
            bodyCollider.isTrigger = false;
            bodyCollider.radius = 0.35f;
        }
    }

    private static void DisablePlayerInput(GameObject playerObject)
    {
        PlayerInput playerInput = playerObject.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;
    }

    private static void ApplySharedComponents(GameObject playerObject)
    {
        if (playerObject.GetComponent<PlayerMovement>() == null)
            playerObject.AddComponent<PlayerMovement>();

        if (playerObject.GetComponent<PlayerStat>() == null)
            playerObject.AddComponent<PlayerStat>();

        if (playerObject.GetComponent<PlayerVisual>() == null)
            playerObject.AddComponent<PlayerVisual>();

        if (playerObject.GetComponent<PlayerController>() == null)
            playerObject.AddComponent<PlayerController>();
    }

    private static void ApplyHubComponents(GameObject playerObject)
    {
        if (playerObject.GetComponent<PlayerInteractor>() == null)
            playerObject.AddComponent<PlayerInteractor>();
    }

    private static void ApplyDungeonComponents(GameObject playerObject)
    {
        if (playerObject.GetComponent<PlayerManager>() == null)
            playerObject.AddComponent<PlayerManager>();

        if (playerObject.GetComponent<PlayerDash>() == null)
            playerObject.AddComponent<PlayerDash>();

        if (playerObject.GetComponent<PlayerInteractor>() == null)
            playerObject.AddComponent<PlayerInteractor>();

        if (playerObject.GetComponent<PlayerHealth>() == null)
            playerObject.AddComponent<PlayerHealth>();

        if (playerObject.GetComponent<PlayerAttack>() == null)
            playerObject.AddComponent<PlayerAttack>();

        if (playerObject.GetComponent<MeleeAttack>() == null)
            playerObject.AddComponent<MeleeAttack>();

        if (playerObject.GetComponent<ProjectileAttack>() == null)
            playerObject.AddComponent<ProjectileAttack>();

        if (playerObject.GetComponent<LootMagnet>() == null)
            playerObject.AddComponent<LootMagnet>();
    }

    private static void ApplyProgress(GameObject playerObject)
    {
        PlayerStat stat = playerObject.GetComponent<PlayerStat>();
        if (stat != null && PlayerData.Instance != null)
            PlayerData.Instance.ApplyTo(stat);

        SaveManager.Instance?.ApplyEquipmentToCurrentPlayer();
    }

    public static void ResetHubTransform(GameObject playerObject)
    {
        if (playerObject == null)
            return;

        playerObject.transform.position = Vector3.zero;

        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
