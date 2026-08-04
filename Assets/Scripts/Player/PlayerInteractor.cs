using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRadius = -1f; // BALANCE: playerInteractRadius when < 0
    [SerializeField] private LayerMask interactLayer = ~0;

    private readonly Collider2D[] overlapResults = new Collider2D[16];
    private IInteractable currentTarget;

    private float InteractRadius => interactRadius > 0f ? interactRadius : GameBalance.PlayerInteractRadius;

    private void Update()
    {
        if (GameStateController.Instance != null && !GameStateController.Instance.CanWorldInteract)
        {
            currentTarget = null;
            InteractionPromptUI.Instance?.ClearPrompt();
            return;
        }

        if (!CanInteractInCurrentScene())
        {
            currentTarget = null;
            InteractionPromptUI.Instance?.ClearPrompt();
            return;
        }

        currentTarget = FindBestInteractable();
        InteractionPromptUI.Instance?.SetPrompt(
            currentTarget != null ? currentTarget.GetPrompt(transform) : string.Empty);

        if (WasSpacePressedThisFrame())
            TryInteract();
    }

    private static bool CanInteractInCurrentScene()
    {
        return GameSceneNames.IsHubScene() || GameSceneNames.IsDungeonScene();
    }

    private bool WasSpacePressedThisFrame()
    {
        return GameInput.WasPressed(GameAction.Interact);
    }

    private IInteractable FindBestInteractable()
    {
        IInteractable bestFromPhysics = FindBestFromPhysics();
        if (bestFromPhysics != null)
            return bestFromPhysics;

        return FindBestFromRegistered();
    }

    private IInteractable FindBestFromPhysics()
    {
        ContactFilter2D filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = interactLayer,
            useTriggers = true
        };

        int count = Physics2D.OverlapCircle(transform.position, InteractRadius, filter, overlapResults);
        IInteractable best = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = overlapResults[i];
            if (hit == null)
                continue;

            IInteractable interactable = hit.GetComponent<IInteractable>() ?? hit.GetComponentInParent<IInteractable>();
            if (interactable == null || !interactable.CanInteract(transform))
                continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance >= bestDistance)
                continue;

            bestDistance = distance;
            best = interactable;
        }

        return best;
    }

    private IInteractable FindBestFromRegistered()
    {
        PortalTrigger[] portals = FindObjectsByType<PortalTrigger>(FindObjectsSortMode.None);
        IInteractable best = null;
        float bestDistance = float.MaxValue;

        foreach (PortalTrigger portal in portals)
        {
            if (portal == null || !portal.CanInteract(transform))
                continue;

            float distance = Vector2.Distance(transform.position, portal.transform.position);
            if (distance >= bestDistance)
                continue;

            bestDistance = distance;
            best = portal;
        }

        return best;
    }

    private void TryInteract()
    {
        if (currentTarget == null || !currentTarget.CanInteract(transform))
            return;

        currentTarget.Interact(transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, InteractRadius);
    }
}
