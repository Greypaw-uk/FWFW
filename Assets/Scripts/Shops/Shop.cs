using UnityEngine;

public class Shop : MonoBehaviour, IShop
{
    private IInventory inventory;
    private ICurrency currency;

    public void Init(IInventory _inventory, ICurrency _currency)
    {
        inventory = _inventory;
        currency = _currency;
    }

    public void SellItem(Items.Item item)
    {
        int payout = Mathf.RoundToInt(item.Weight * item.PricePerKg);
        currency.AddMoney(payout);
        inventory.RemoveItem(item);
    }
}