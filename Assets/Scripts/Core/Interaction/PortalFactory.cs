using UnityEngine;

public static class PortalFactory
{
    public static PortalTrigger CreateFromData(PortalData data, string objectName, Vector3 position)
    {
        if (data == null)
            return null;

        GameObject portalObject = new GameObject(objectName);
        portalObject.transform.position = position;

        SpriteRenderer renderer = portalObject.AddComponent<SpriteRenderer>();
        renderer.sprite = ProceduralSpriteFactory.CreateCircle(data.placeholderColor);
        renderer.sortingOrder = 1;

        BoxCollider2D collider = portalObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.4f, 1.4f);

        PortalTrigger portal = portalObject.AddComponent<PortalTrigger>();
        portal.ApplyData(data);

        CreateWorldLabel(portalObject.transform, data.displayName);

        return portal;
    }

    private static void CreateWorldLabel(Transform parent, string text)
    {
        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(parent, false);
        labelObject.transform.localPosition = new Vector3(0f, 0.9f, 0f);

        TextMesh textMesh = labelObject.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 28;
        textMesh.characterSize = 0.07f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
    }
}
