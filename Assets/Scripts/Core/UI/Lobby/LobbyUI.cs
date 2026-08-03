using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button enterDungeonButton;
    [SerializeField] private Button resetSaveButton;

    private void Awake()
    {
        AutoWireReferences();
    }

    private void Start()
    {
        RefreshStatus();
    }

    private void OnEnable()
    {
        RefreshStatus();
    }

    private void AutoWireReferences()
    {
        if (statusText == null)
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                if (text.gameObject.name == "StatusText")
                {
                    statusText = text;
                    break;
                }
            }
        }

        if (enterDungeonButton == null)
        {
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == "Button")
                {
                    enterDungeonButton = button;
                    break;
                }
            }
        }

        if (enterDungeonButton != null)
        {
            enterDungeonButton.onClick.RemoveListener(OnClickEnterDungeon);
            enterDungeonButton.onClick.AddListener(OnClickEnterDungeon);
        }

        if (resetSaveButton != null)
        {
            resetSaveButton.onClick.RemoveListener(OnClickResetSave);
            resetSaveButton.onClick.AddListener(OnClickResetSave);
        }
    }

    public void BindStatusText(TMP_Text text)
    {
        statusText = text;
    }

    public void BindEnterButton(Button button)
    {
        enterDungeonButton = button;
        AutoWireReferences();
    }

    public void RefreshStatus()
    {
        if (statusText == null || PlayerData.Instance == null)
            return;

        statusText.text =
            $"Gold: {PlayerData.Instance.Gold}    " +
            $"Lv.{PlayerData.Instance.Level}    " +
            $"EXP: {PlayerData.Instance.CurrentExp:F0}";
    }

    public void OnClickEnterDungeon()
    {
        if (DungeonManager.Instance == null)
            new GameObject("DungeonManager").AddComponent<DungeonManager>();

        DungeonManager.Instance.ResetRunStats();
        DungeonManager.Instance.EnterDungeon();
    }

    public void OnClickResetSave()
    {
        SaveManager.Instance?.ResetSave();

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.Gold = 0;
            PlayerData.Instance.Level = 1;
            PlayerData.Instance.CurrentExp = 0;
            PlayerData.Instance.StatPoints = 0;
            PlayerData.Instance.PassivePoints = 0;
        }

        RefreshStatus();
    }
}
