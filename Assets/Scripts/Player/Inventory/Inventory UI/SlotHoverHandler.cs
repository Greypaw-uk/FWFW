using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Tooltip tooltip;
    private string itemName;
    private string itemWeight;
    private string itemPrice;

    public void Init(Tooltip tooltipRef, string name, string weight, string price)
    {
        tooltip = tooltipRef;
        itemName = name;
        itemWeight = weight;
        itemPrice = price;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null && !string.IsNullOrEmpty(itemName))
            tooltip.Show($"{itemName} - {itemWeight}kg - ${itemPrice}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.Hide();
    }
}


