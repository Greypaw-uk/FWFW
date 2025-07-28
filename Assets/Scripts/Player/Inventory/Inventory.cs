using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<Items.Item> inventoryItems = new();

    [SerializeField] private List<FishSpriteData> fishSprites;

    public void AddItem(string name, float weight, float price)
    {
        Items.Item item = new Items.Item
        {
            Name = name,
            Weight = weight,
            Price = price
        };

        inventoryItems.Add(item);
    }

    public List<Items.Item> GetItems()
    {
        return inventoryItems;
    }

    public Sprite GetSpriteForItem(string itemName)
    {
        var match = fishSprites.Find(x => x.name == itemName);
        return match?.sprite;
    }
}

[System.Serializable]
public class FishSpriteData
{
    public string name;
    public Sprite sprite;
}