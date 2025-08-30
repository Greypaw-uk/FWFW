using System.Collections.Generic;
using UnityEngine;

public interface IInventory
{
    void AddItem(Items.Item item);
    void RemoveItem(Items.Item item);
    List<Items.Item> GetItems();
    Sprite GetSpriteForItem(string itemName);
    int GetMaxItems();
    int GetCurrentItemCount();
}