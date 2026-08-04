using System;
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveKey = "MiniProject_PlayerSave";

    private EquipmentSaveUtility.SavedEquipmentSlot[] loadedEquipment = Array.Empty<EquipmentSaveUtility.SavedEquipmentSlot>();

    [Serializable]
    public class SavedInventorySlot
    {
        public int itemId;
        public int quantity;
    }

    [Serializable]
    public class SaveData
    {
        public int gold;
        public int level = 1;
        public float currentExp;
        public int statPoints;
        public int passivePoints;
        public SavedInventorySlot[] inventory = Array.Empty<SavedInventorySlot>();
        public EquipmentSaveUtility.SavedEquipmentSlot[] equipment = Array.Empty<EquipmentSaveUtility.SavedEquipmentSlot>();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Load();
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    public void Save()
    {
        if (PlayerData.Instance == null)
            return;

        if (PlayerStat.Instance != null)
            PlayerData.Instance.SaveFrom(PlayerStat.Instance);

        SaveData saveData = new SaveData
        {
            gold = PlayerData.Instance.Gold,
            level = PlayerData.Instance.Level,
            currentExp = PlayerData.Instance.CurrentExp,
            statPoints = PlayerData.Instance.StatPoints,
            passivePoints = PlayerData.Instance.PassivePoints,
            inventory = CaptureInventory(),
            equipment = CaptureEquipment()
        };

        string json = JsonConvert.SerializeObject(saveData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private static SaveData DeserializeSaveData(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<SaveData>(json);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"SaveManager: Newtonsoft load failed, trying legacy JsonUtility. {exception.Message}");
            return JsonUtility.FromJson<SaveData>(json);
        }
    }

    public void Load()
    {
        if (PlayerData.Instance == null)
            return;

        if (!HasSave())
        {
            StarterInventorySeeder.SeedIfEmpty();
            return;
        }

        SaveData saveData = DeserializeSaveData(PlayerPrefs.GetString(SaveKey));

        if (saveData == null)
        {
            StarterInventorySeeder.SeedIfEmpty();
            return;
        }

        PlayerData.Instance.Gold = saveData.gold;
        PlayerData.Instance.Level = saveData.level;
        PlayerData.Instance.CurrentExp = saveData.currentExp;
        PlayerData.Instance.StatPoints = saveData.statPoints;
        PlayerData.Instance.PassivePoints = saveData.passivePoints;

        ApplyInventory(saveData.inventory);
        ApplyEquipment(saveData.equipment);
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }

    public void ResetAllProgress()
    {
        ResetSave();

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.Gold = 0;
            PlayerData.Instance.Level = 1;
            PlayerData.Instance.CurrentExp = 0;
            PlayerData.Instance.StatPoints = 0;
            PlayerData.Instance.PassivePoints = 0;
        }

        Inventory.Instance?.Clear();
        ClearEquippedItems();
        loadedEquipment = Array.Empty<EquipmentSaveUtility.SavedEquipmentSlot>();
        StarterInventorySeeder.Reset();
        StarterInventorySeeder.SeedIfEmpty();
    }

    public void ApplyEquipmentToCurrentPlayer()
    {
        if (PlayerStat.Instance == null)
            return;

        EquipmentSaveUtility.Apply(PlayerStat.Instance, loadedEquipment);
    }

    private static SavedInventorySlot[] CaptureInventory()
    {
        Inventory inventory = Inventory.Instance;
        if (inventory == null)
            return Array.Empty<SavedInventorySlot>();

        SavedInventorySlot[] slots = new SavedInventorySlot[inventory.GetFilledSlotCount()];
        int index = 0;

        foreach (InventorySlot slot in inventory.Slots)
        {
            if (slot.IsEmpty || slot.item.data == null)
                continue;

            slots[index++] = new SavedInventorySlot
            {
                itemId = slot.item.data.id,
                quantity = slot.item.quantity
            };
        }

        return slots;
    }

    private static void ApplyInventory(SavedInventorySlot[] savedSlots)
    {
        Inventory inventory = Inventory.Instance;
        if (inventory == null)
            return;

        inventory.Clear(silent: true);

        if (savedSlots == null || savedSlots.Length == 0)
        {
            StarterInventorySeeder.SeedIfEmpty();
            return;
        }

        ItemCatalog.EnsureDefaults();

        foreach (SavedInventorySlot savedSlot in savedSlots)
        {
            if (savedSlot == null || savedSlot.quantity <= 0)
                continue;

            ItemData item = ItemCatalog.GetById(savedSlot.itemId);
            if (item == null)
                continue;

            inventory.AddItem(item, savedSlot.quantity);
        }

        inventory.NotifyChanged();
    }

    private static EquipmentSaveUtility.SavedEquipmentSlot[] CaptureEquipment()
    {
        EquipmentSaveUtility.SavedEquipmentSlot[] captured = EquipmentSaveUtility.Capture(PlayerStat.Instance);
        if (Instance != null)
            Instance.loadedEquipment = captured;
        return captured;
    }

    private static void ApplyEquipment(EquipmentSaveUtility.SavedEquipmentSlot[] savedEquipment)
    {
        if (Instance == null)
            return;

        Instance.loadedEquipment = savedEquipment ?? Array.Empty<EquipmentSaveUtility.SavedEquipmentSlot>();
        Instance.ApplyEquipmentToCurrentPlayer();
    }

    private static void ClearEquippedItems()
    {
        PlayerStat.Instance?.ClearAllEquipment();
    }
}
