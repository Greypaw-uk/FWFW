using UnityEngine;
using UnityEngine.EventSystems;

public class SlotRightClickHandler : MonoBehaviour, IPointerClickHandler
{
    private ContextMenu contextMenu;
    private Inventory inventory;
    private Items.Item item;

    public void Init(ContextMenu contextMenu, Inventory inventory, Items.Item item)
    {
        this.contextMenu = contextMenu;
        this.inventory = inventory;
        this.item = item;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            contextMenu.Show(Input.mousePosition, inventory, item);
        }
    }
}
