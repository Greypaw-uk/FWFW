using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Items.Item item;
    private IShop shopLogic;
    private ShopUI shopUI;

    public void Init(Items.Item _item, IShop _shop, ShopUI _shopUI)
    {
        item = _item;
        shopLogic = _shop;
        shopUI = _shopUI;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        shopLogic.SellItem(item);   // GAME LOGIC
        shopUI.RefreshUI();        // UI refresh or re-open
    }
}