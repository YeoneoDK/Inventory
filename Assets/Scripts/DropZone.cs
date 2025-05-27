using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public EquipmentSlot allowedSlot;
    public bool isVaultSlot;
    public EquipmentDisplay equipmentDisplay;

    private void Start()
    {
        equipmentDisplay = FindObjectOfType<EquipmentDisplay>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Ensure draggedItem is being accessed correctly from eventData
        DraggableItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();

        // Check if draggedItem and its equipmentItem are valid
        if (draggedItem == null || draggedItem.equipmentItem == null)
        {
            Debug.LogError("Invalid dragged item or its equipment data is null!");
            return; // Exit if dragged item or its data is null
        }
        EquipmentItem draggedData = draggedItem.equipmentItem;

        Transform oldParent = draggedItem.originalParent;
        DropZone oldDropZone = oldParent?.GetComponent<DropZone>();
        bool cameFromVault = oldDropZone != null && oldDropZone.isVaultSlot;
        bool cameFromEquipment = oldDropZone != null && !oldDropZone.isVaultSlot;

        Debug.Log($"OnDrop triggered: From {(cameFromVault ? "Vault" : "Equipment")} to {(isVaultSlot ? "Vault" : "Equipment")}");
        // Handle different drop scenarios
        if (isVaultSlot)
        {
            if (cameFromVault)
            {
                // Vault to Vault logic
                VaultSlot targetVaultSlot = GetComponent<VaultSlot>();
                if (targetVaultSlot == null) return;

                EquipmentItem existingItem = targetVaultSlot.GetItem();
                targetVaultSlot.SetItem(draggedData);

                VaultSlot originVault = oldParent.GetComponent<VaultSlot>();
                if (originVault != null)
                    originVault.SetItem(existingItem);
            }
            else if (cameFromEquipment)
            {
                // Equipment to Vault logic
                VaultSlot targetVaultSlot = GetComponent<VaultSlot>();
                if (targetVaultSlot == null) return;

                EquipmentItem existingItem = targetVaultSlot.GetItem();
                targetVaultSlot.SetItem(draggedData);

                equipmentDisplay.UpdateEquipmentSlot(oldDropZone.allowedSlot, existingItem);
            }
        }
        else
        {
            // Equipment slot logic
            if (draggedData.slot != allowedSlot) return;

            EquipmentItem currentlyEquipped = ItemDatabase.GetItemByCode(
                equipmentDisplay.currentCharacter != null ?
                GetCodeBySlot(allowedSlot) : -1);

            equipmentDisplay.UpdateEquipmentSlot(allowedSlot, draggedData);

            VaultSlot originVault = oldParent.GetComponent<VaultSlot>();
            if (originVault != null)
                originVault.SetItem(currentlyEquipped);
        }
    }



    private int GetCodeBySlot(EquipmentSlot slot)
    {
        var c = equipmentDisplay.currentCharacter;
        return slot switch
        {
            EquipmentSlot.Weapon => c.weapon,
            EquipmentSlot.Head => c.head,
            EquipmentSlot.Top => c.top,
            EquipmentSlot.Bottom => c.bottom,
            EquipmentSlot.Shoes => c.shoes,
            EquipmentSlot.Ring => c.ring,
            EquipmentSlot.Necklace => c.necklace,
            EquipmentSlot.Bracelet => c.bracelet,
            _ => -1
        };
    }
}