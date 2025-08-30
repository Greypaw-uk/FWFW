using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour, IInventory
{
    // Store any type of IItem
    public List<Items.Item> inventoryItems = new();

    [SerializeField] private List<FishSpriteData> itemSprites;

    [SerializeField] private int maxItems = 10;
    [SerializeField] private InventoryUI InventoryUI;

    public void AddItem(Items.Item item)
    {
        inventoryItems.Add(item);
        InventoryUI.RefreshInventory();
    }


    public void RemoveItem(Items.Item item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
            InventoryUI.RefreshInventory();
        }
        else
            Debug.LogWarning($"[Inventory] Item {item.Name} not found in inventory.");
    }


    public List<Items.Item> GetItems()
    {
        return inventoryItems;
    }


    public int GetMaxItems() => maxItems;
    public int GetCurrentItemCount() => inventoryItems.Count;


    /// <summary>
    /// Get sprite for item by name - stored in player prefab under attached inventory.cs
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public Sprite GetSpriteForItem(string itemName)
    {
        var match = itemSprites.Find(x => x.name == itemName);
        return match?.sprite;
    }

    public void SetUI(InventoryUI ui)
    {
        InventoryUI = ui;
    }
}



[System.Serializable]
public class FishSpriteData
{
    public string name;
    public Sprite sprite;
}
