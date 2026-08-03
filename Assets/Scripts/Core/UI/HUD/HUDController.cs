using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("Bars")]
    [SerializeField] private Slider hpBar;

    [Header("Texts")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text inventoryText;
    [SerializeField] private TMP_Text damageFeedbackText;

    [Header("Damage Feedback")]
    [SerializeField] private float damageFeedbackDuration = 1.2f;

    private float damageFeedbackTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        AutoWireReferences();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void AutoWireReferences()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        TMP_Text[] texts = canvas.GetComponentsInChildren<TMP_Text>(true);
        Slider[] sliders = canvas.GetComponentsInChildren<Slider>(true);

        if (hpBar == null)
        {
            foreach (Slider slider in sliders)
            {
                if (slider.gameObject.name == "HPBar")
                {
                    hpBar = slider;
                    break;
                }
            }
        }

        foreach (TMP_Text text in texts)
        {
            switch (text.gameObject.name)
            {
                case "LevelText":
                    if (levelText == null) levelText = text;
                    break;
                case "KillText":
                case "ExpText":
                    if (killText == null) killText = text;
                    break;
                case "GoldText":
                    if (goldText == null) goldText = text;
                    break;
                case "InventoryText":
                    if (inventoryText == null) inventoryText = text;
                    break;
                case "DamageFeedbackText":
                    if (damageFeedbackText == null) damageFeedbackText = text;
                    break;
            }
        }
    }

    private void Update()
    {
        RefreshStats();
        UpdateDamageFeedback();
    }

    private void RefreshStats()
    {
        if (PlayerManager.Instance?.Health != null && PlayerStat.Instance != null)
        {
            float maxHp = PlayerStat.Instance.MaxHp;
            if (hpBar != null && maxHp > 0f)
                hpBar.value = PlayerManager.Instance.Health.CurrentHealth / maxHp;
        }

        if (levelText != null && PlayerStat.Instance != null)
        {
            levelText.text =
                $"Lv.{PlayerStat.Instance.CurrentLevel}  " +
                $"EXP {PlayerStat.Instance.CurrentExp:F0}/{PlayerStat.Instance.MaxExp:F0}";
        }

        if (killText != null && DungeonManager.Instance != null)
            killText.text = $"Kills: {DungeonManager.Instance.KilledMonsters}";

        if (goldText != null && PlayerStat.Instance != null)
            goldText.text = $"Gold: {PlayerStat.Instance.Gold}";

        if (inventoryText != null && Inventory.Instance != null)
            inventoryText.text = $"Items: {Inventory.Instance.GetFilledSlotCount()} ({Inventory.Instance.GetTotalQuantity()})";
    }

    public void ShowDamageTaken(float damage)
    {
        if (damageFeedbackText == null)
            return;

        damageFeedbackText.text = $"-{Mathf.RoundToInt(damage)}";
        damageFeedbackText.color = new Color(1f, 0.35f, 0.35f, 1f);
        damageFeedbackTimer = damageFeedbackDuration;
    }

    private void UpdateDamageFeedback()
    {
        if (damageFeedbackText == null || damageFeedbackTimer <= 0f)
            return;

        damageFeedbackTimer -= Time.deltaTime;
        float alpha = Mathf.Clamp01(damageFeedbackTimer / damageFeedbackDuration);
        Color color = damageFeedbackText.color;
        damageFeedbackText.color = new Color(color.r, color.g, color.b, alpha);
    }
}
