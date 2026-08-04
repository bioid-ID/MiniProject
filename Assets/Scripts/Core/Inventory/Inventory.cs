using System;

using System.Collections.Generic;

using UnityEngine;



public class Inventory : MonoBehaviour

{

    public static Inventory Instance { get; private set; }



    public event Action Changed;



    [SerializeField]

    private int slotCount = 100;



    public List<InventorySlot> Slots { get; private set; }



    private void Awake()

    {

        if (Instance != null)

        {

            Destroy(this);

            return;

        }



        Instance = this;

        DontDestroyOnLoad(gameObject);



        Slots = new List<InventorySlot>();



        for (int i = 0; i < slotCount; i++)

            Slots.Add(new InventorySlot());

    }



    private void OnDestroy()

    {

        if (Instance == this)

            Instance = null;

    }



    public bool AddItem(ItemData item, int amount = 1)

    {

        if (item == null)

            return false;



        if (item.stackable)

        {

            foreach (InventorySlot slot in Slots)

            {

                if (slot.IsEmpty)

                    continue;



                if (slot.item.data != item)

                    continue;



                slot.item.quantity += amount;

                NotifyChanged();

                return true;

            }

        }



        foreach (InventorySlot slot in Slots)

        {

            if (!slot.IsEmpty)

                continue;



            slot.item = new InventoryItem(item, amount);

            NotifyChanged();

            return true;

        }



        return false;

    }



    public void RemoveItem(ItemData item, int amount = 1)

    {

        foreach (InventorySlot slot in Slots)

        {

            if (slot.IsEmpty)

                continue;



            if (slot.item.data != item)

                continue;



            slot.item.quantity -= amount;



            if (slot.item.quantity <= 0)

                slot.Clear();



            NotifyChanged();

            return;

        }

    }



    public bool HasItem(ItemData item, int amount)

    {

        int count = 0;



        foreach (InventorySlot slot in Slots)

        {

            if (slot.IsEmpty)

                continue;



            if (slot.item.data != item)

                continue;



            count += slot.item.quantity;

        }



        return count >= amount;

    }



    public void Clear(bool silent = false)

    {

        foreach (InventorySlot slot in Slots)

            slot.Clear();



        if (!silent)

            NotifyChanged();

    }



    public void NotifyChanged()

    {

        Changed?.Invoke();

    }



    public int GetFilledSlotCount()

    {

        int count = 0;



        foreach (InventorySlot slot in Slots)

        {

            if (!slot.IsEmpty)

                count++;

        }



        return count;

    }



    public int GetTotalQuantity()

    {

        int total = 0;



        foreach (InventorySlot slot in Slots)

        {

            if (slot.IsEmpty)

                continue;



            total += slot.item.quantity;

        }



        return total;

    }

}


