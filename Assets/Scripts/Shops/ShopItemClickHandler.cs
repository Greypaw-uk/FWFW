using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Items.Item item;
    private IShop shopLogic;
    private ShopUI shopUI;


    /// <summary>
    /// Initializes the click handler with the item, shop logic, and shop UI references.
    /// </summary>
    /// <param name="_item"></param>
    /// <param name="_shop"></param>
    /// <param name="_shopUI"></param>
    public void Init(Items.Item _item, IShop _shop, ShopUI _shopUI)
    {
        item = _item;
        shopLogic = _shop;
        shopUI = _shopUI;
    }


    /// <summary>
    /// Handles pointer click events to sell the item.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopLogic != null && item != null)
        {
            shopLogic.SellItem(item);  
            shopUI.RefreshUI();        
        }
    }
}