using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image itemIcon;
    public Transform originalParent;
    private Vector2 originalPosition;
    public EquipmentItem equipmentItem;

    public void Initialize(EquipmentItem item)
    {
        equipmentItem = item;
    }

    private void Start()
    {
        if (itemIcon == null)
        {
            itemIcon = GetComponent<Image>();
            if (itemIcon == null)
            {
                Debug.LogError("DraggableItem is missing an Image component.");
                return;
            }
        }
        ResizeIcon();
    }

    private void ResizeIcon()
    {
        if (itemIcon != null)
        {
            RectTransform iconRect = itemIcon.GetComponent<RectTransform>();
            if (iconRect != null)
            {
                iconRect.sizeDelta = new Vector2(100f, 100f);
                LayoutRebuilder.ForceRebuildLayoutImmediate(iconRect);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        transform.SetParent(transform.root);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // Log to confirm the drag operation starts
        Debug.Log($"OnBeginDrag: {gameObject.name}, originalParent: {originalParent.name}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"OnEndDrag: {gameObject.name}");

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        DropZone dropZone = eventData.pointerEnter?.GetComponent<DropZone>();
        if (dropZone != null)
        {
            Debug.Log("Trying to drop on: " + eventData.pointerEnter?.name);
            ExecuteEvents.Execute(dropZone.gameObject, eventData, ExecuteEvents.dropHandler); // Manually call OnDrop
        }
        else
        {
            // If no valid drop zone, return the item to the original parent.
            transform.SetParent(originalParent);
            transform.position = originalPosition;
            Debug.Log("Item returned to original position.");
        }
    }
}

