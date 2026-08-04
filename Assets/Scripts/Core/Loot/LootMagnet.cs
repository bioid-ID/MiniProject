using UnityEngine;

/// <summary>
/// Legacy helper. Loot magnet pull is handled by <see cref="LootPhysicsBehavior"/> on each loot instance.
/// Kept for compatibility if attached to the player in a scene.
/// </summary>
public class LootMagnet : MonoBehaviour
{
    [SerializeField] private float radius = 4.5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
