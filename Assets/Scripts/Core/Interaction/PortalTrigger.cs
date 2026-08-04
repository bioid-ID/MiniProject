using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PortalTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private PortalData portalData;
    [SerializeField] private float interactRange = -1f; // BALANCE: portalInteractRange when < 0

    public PortalFlow Flow => portalData != null ? portalData.flow : PortalFlow.EnterDungeon;
    public string PortalId => portalData != null ? portalData.portalId : string.Empty;

    private float InteractRange => interactRange > 0f ? interactRange : GameBalance.PortalInteractRange;

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

        string keyLabel = GameKeyBindings.GetDisplayName(GameKeyBindings.Interact);

        return portalData.flow == PortalFlow.ReturnToHub
            ? $"[{keyLabel}] Return to {portalData.displayName}"
            : $"[{keyLabel}] Enter {portalData.displayName}";
    }

    public bool CanInteract(Transform interactor)
    {
        if (interactor == null || portalData == null)
            return false;

        if (GameStateController.Instance != null && !GameStateController.Instance.CanWorldInteract)
            return false;

        return Vector2.Distance(interactor.position, transform.position) <= InteractRange;
    }

    public void Interact(Transform interactor)
    {
        if (!CanInteract(interactor) || portalData == null || DungeonManager.Instance == null)
            return;

        GameStateController.Instance?.PrepareSceneTransition();

        if (portalData.flow == PortalFlow.ReturnToHub)
        {
            InteractionPromptUI.Instance?.ClearPrompt();
            GameFeel.PortalEnter();
            DungeonManager.Instance.EscapePortalDirect();
            return;
        }

        if (portalData.resetDungeonRun)
            DungeonManager.Instance.ResetRunStats();

        string resolvedSceneName = SceneBuildUtility.ResolveSceneName(portalData.targetSceneName);

        if (SceneLoader.Instance != null && !SceneLoader.Instance.CanLoadScene(resolvedSceneName))
        {
            Debug.LogError($"Portal '{portalData.displayName}' target scene is missing from Build Settings: {portalData.targetSceneName}");
            return;
        }

        GameFeel.PortalEnter();
        DungeonManager.Instance.EnterDungeon(resolvedSceneName);
    }
}
