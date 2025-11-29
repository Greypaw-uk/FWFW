using UnityEngine;

public class Shop : MonoBehaviour, IShop
{
    private IInventory inventory;
    private ICurrency currency;


    /// <summary>
    /// Initializes the shop with player inventory and currency references.
    /// </summary>
    /// <param name="_inventory"></param>
    /// <param name="_currency"></param>
    public void Init(IInventory _inventory, ICurrency _currency)
    {
        inventory = _inventory;
        currency = _currency;

        Debug.Log($"[Shop] Init Currency object: {currency.GetHashCode()}");
    }


    /// <summary>
    /// Sells an item from the player's inventory to the shop, providing currency based on weight.
    /// </summary>
    public void SellItem(Items.Item item)
    {
        Debug.Log($"[Shop] Selling {item.Name}, weighing {item.Weight}, valued at {item.Price}.");


        Debug.Log($"[Shop] Selling item '{item.Name}' for {item.Price}.");
        currency.AddMoney(Mathf.FloorToInt(item.Price));
        inventory.RemoveItem(item);
    }
}