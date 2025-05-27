using UnityEngine;
using System.Collections.Generic;

public class VaultUI : MonoBehaviour
{
    public List<VaultSlot> vaultSlots; // Assign in the Inspector (fixed slots)

    public void Start()
    {
        PopulateVault();
    }

    public void PopulateVault()
    {
        List<int> vaultItems = GameManager.instance.gameData.equipmentVaultCodes;

        if (vaultItems.Count == 0)
        {
            Debug.LogWarning("No items found in vault.");
        }

        int i = 0;

        // Iterate through the slots and try to populate each one
        foreach (VaultSlot slot in vaultSlots)
        {
            if (i < vaultItems.Count)
            {
                int itemcode = vaultItems[i];

                if (itemcode != -1)
                {
                    EquipmentItem item = ItemDatabase.GetItemByCode(itemcode); // Convert EquipmentData to EquipmentItem
                    slot.SetItem(item);  // Now passing EquipmentItem
                }
                else
                {
                    Debug.LogWarning($"Item at index {i} is null.");
                    slot.SetItem(null); // Empty slot
                }

                i++;  // Increment to the next item
            }
            else
            {
                slot.SetItem(null);  // Empty slot
                i++;
            }
        }
    }

}