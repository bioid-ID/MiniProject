using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PortalTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private PortalData portalData;
    [SerializeField] private float extraInteractPadding = 0.35f;

    public PortalFlow Flow => portalData != null ? portalData.flow : PortalFlow.EnterDungeon;
    public string PortalId => portalData != null ? portalData.portalId : string.Empty;

    private void Reset()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }

    public void ApplyData(PortalData data)
    {
        portalData = data;
    }

    public string GetPrompt(Transform interactor)
    {
        if (!CanInteract(interactor) || portalData == null)
            return string.Empty;

        return portalData.flow == PortalFlow.ReturnToHub
            ? $"[Space] Return to {portalData.displayName}"
            : $"[Space] Enter {portalData.displayName}";
    }

    public bool CanInteract(Transform interactor)
    {
        if (interactor == null || portalData == null)
            return false;

        if (GameStateController.Instance != null && !GameStateController.Instance.CanWorldInteract)
            return false;

        Collider2D portalCollider = GetComponent<Collider2D>();
        if (portalCollider != null && portalCollider.OverlapPoint(interactor.position))
            return true;

        float range = 1f;
        if (portalCollider != null)
            range = Mathf.Max(portalCollider.bounds.extents.x, portalCollider.bounds.extents.y) + extraInteractPadding;

        return Vector2.Distance(interactor.position, transform.position) <= range;
    }

    public void Interact(Transform interactor)
    {
        if (!CanInteract(interactor) || portalData == null || DungeonManager.Instance == null)
            return;

        GameStateController.Instance?.PrepareSceneTransition();

        if (portalData.flow == PortalFlow.ReturnToHub)
        {
            DungeonManager.Instance.EscapeToHub();
            return;
        }

        if (portalData.resetDungeonRun)
            DungeonManager.Instance.ResetRunStats();

        if (SceneLoader.Instance != null && !SceneLoader.Instance.CanLoadScene(portalData.targetSceneName))
        {
            Debug.LogError($"Portal '{portalData.displayName}' target scene is missing from Build Settings: {portalData.targetSceneName}");
            return;
        }

        DungeonManager.Instance.EnterDungeon(portalData.targetSceneName);
    }
}
