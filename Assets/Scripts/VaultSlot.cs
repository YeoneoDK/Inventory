using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VaultSlot : MonoBehaviour
{
    public Image itemIcon;
    public EquipmentItem storedItem;
    public RectTransform slotRectTransform;
    private DraggableItem draggableItem;
    private Vector2 defaultIconSize;

    private void Awake()
    {
        defaultIconSize = itemIcon.rectTransform.sizeDelta;
        draggableItem = GetComponentInChildren<DraggableItem>();
    }

    public void SetItem(EquipmentItem item)
    {
        if (item == null)
        {
            itemIcon.sprite = null;
            itemIcon.color = new Color(1, 1, 1, 0);
            storedItem = null;
            return;
        }

        storedItem = item;
        itemIcon.sprite = item.icon;
        itemIcon.color = Color.white;

        if (draggableItem != null)
        {
            draggableItem.Initialize(item);
        }
    }
    public EquipmentItem GetItem()
    {
        return storedItem;
    }
}