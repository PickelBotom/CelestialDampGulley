using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public Item item;
    public int count;

    public void Copy(ItemSlot slot)
    {
        item = slot.item;
        count = slot.count;
    }

    public void Set(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public void Clear()
    {
        item = null;
        count = 0;
    }
}

[CreateAssetMenu(menuName = "Data/Item Container")]
public class ItemContainer : ScriptableObject
{
    public List<ItemSlot> slots;

    public void Add(Item item, int count = 1)
    {
        if (item == null)
        {
            Debug.LogError("Attempting to add a null item to the inventory.");
            return;
        }

        Debug.Log($"Adding item to inventory: {item.Name}, Count: {count}");

        if (item.stackable)
        {
            ItemSlot itemSlot = slots.Find(x => x.item == item);
            if (itemSlot != null)
            {
                itemSlot.count += count;
            }
            else
            {
                itemSlot = slots.Find(x => x.item == null);
                if (itemSlot != null)
                {
                    itemSlot.item = item;
                    itemSlot.count = count;
                }
            }
        }
        else
        {
            ItemSlot itemSlot = slots.Find(x => x.item == null);
            if (itemSlot != null)
            {
                itemSlot.item = item;
                itemSlot.count = count;
            }
        }
    }

    public void RemoveItem(Item ItemToRemove, int count = 1)
    {
        if (ItemToRemove.stackable)
        {
            ItemSlot itemSlot = slots.Find(x => x.item == ItemToRemove);
            if (itemSlot == null) return;
            itemSlot.count -= count;
            if (itemSlot.count <= 0)
            {
                itemSlot.Clear();
            }
        }
        else
        {
            while (count > 0)
            {
                count -= 1;
                ItemSlot itemSlot = slots.Find(x => x.item == ItemToRemove);
                if (itemSlot == null) return;
                itemSlot.Clear();
            }
        }
    }

    public bool HasItem(Item item, int count)
    {
        ItemSlot slot = slots.Find(x => x.item == item);
        return slot != null && slot.count >= count;
    }

    public void LoadItemsFromDatabase(DatabaseManager dbManager)
    {
        List<Item> itemsFromDB = DatabaseManager.instance.GetAllItems();
        slots.Clear();
        foreach (var item in itemsFromDB)
        {
            ItemSlot itemSlot = new ItemSlot { item = item, count = 1 };
            slots.Add(itemSlot);
        }
    }
}
