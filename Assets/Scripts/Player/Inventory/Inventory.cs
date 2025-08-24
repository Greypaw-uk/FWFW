using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    // Store any type of IItem
    public List<Items.Item> inventoryItems = new();

    [SerializeField] private List<FishSpriteData> itemSprites;

    public void AddItem(Items.Item item)
    {
        inventoryItems.Add(item);
    }


    public void RemoveItem(Items.Item item)
    {
        if (inventoryItems.Contains(item))
            inventoryItems.Remove(item);
        else
            Debug.LogWarning($"[Inventory] Item {item.Name} not found in inventory.");
    }


    public List<Items.Item> GetItems()
    {
        return inventoryItems;
    }


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
}


[System.Serializable]
public class FishSpriteData
{
    public string name;
    public Sprite sprite;
}
