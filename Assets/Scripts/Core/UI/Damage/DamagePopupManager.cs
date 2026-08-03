using TMPro;
using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance { get; private set; }

    [Header("Colors")]
    [SerializeField] private Color playerHitColor = new Color(1f, 0.35f, 0.35f);
    [SerializeField] private Color enemyHitColor = new Color(1f, 0.95f, 0.4f);
    [SerializeField] private Color criticalColor = new Color(1f, 0.5f, 0.1f);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static void Show(Vector3 worldPosition, float damage, bool isEnemyTarget, bool isCritical = false)
    {
        if (Instance == null)
            return;

        Instance.Spawn(worldPosition, damage, isEnemyTarget, isCritical);
    }

    private void Spawn(Vector3 worldPosition, float damage, bool isEnemyTarget, bool isCritical)
    {
        if (damage <= 0f)
            return;

        GameObject popupObject = new GameObject("DamagePopup");
        popupObject.transform.position = worldPosition + Vector3.up * 0.35f;

        TextMeshPro textMesh = popupObject.AddComponent<TextMeshPro>();
        textMesh.text = Mathf.RoundToInt(damage).ToString();
        textMesh.fontSize = isCritical ? 5f : 4f;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.sortingOrder = 20;

        if (TMP_Settings.defaultFontAsset != null)
            textMesh.font = TMP_Settings.defaultFontAsset;

        if (isCritical)
            textMesh.color = criticalColor;
        else if (isEnemyTarget)
            textMesh.color = enemyHitColor;
        else
            textMesh.color = playerHitColor;

        popupObject.AddComponent<FloatingDamageText>();
    }
}
