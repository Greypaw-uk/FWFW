using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour, IInventory
{
    public List<Items.Item> inventoryItems = new();

    [SerializeField] private List<FishSpriteData> itemSprites;

    [SerializeField] private int maxItems = 10;

    // Event to notify listeners of changes
    public event System.Action OnInventoryChanged;


    public void AddItem(Items.Item item)
    {
        inventoryItems.Add(item);
        OnInventoryChanged?.Invoke();
    }


    public void RemoveItem(Items.Item item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
            OnInventoryChanged?.Invoke();
        }
    }


    public void SellItem(Items.Item item)
    {
        Currency currency = GetComponent<Currency>();
        currency.AddMoney(Mathf.FloorToInt(item.Price));
        
        RemoveItem(item);
    }


    public List<Items.Item> GetItems() => inventoryItems;

    public int GetCurrentItemsCount() => inventoryItems.Count;

    public int GetMaxItemsCount() => maxItems;


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
