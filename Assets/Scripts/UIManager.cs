using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public CharacterInfoDisplay characterInfo;
    public EquipmentDisplay equipmentDisplay;
    public List<VaultSlot> vaultSlots = new List<VaultSlot>(); // Vault UI slots

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void UpdateCharacterUI()
    {
        if (characterInfo != null)
            characterInfo.ShowCharacterInfo();

        if (equipmentDisplay != null)
            equipmentDisplay.UpdateEquipmentDisplay();
    }

    public void UpdateVaultUI()
    {
        List<int> vaultItems = GameManager.instance.gameData.equipmentVaultCodes;

        for (int i = 0; i < vaultSlots.Count; i++)
        {
            if (i < vaultItems.Count)
            {
                EquipmentItem item = ItemDatabase.GetItemByCode(vaultItems[i]);
                if (item != null)
                {
                    vaultSlots[i].SetItem(item); // Correct type
                }
                else
                {
                    Debug.LogWarning($"No EquipmentItem found for code {vaultItems[i]}");
                    vaultSlots[i].SetItem(null); // Empty slot
                }
            }
            else
            {
                vaultSlots[i].SetItem(null); // Empty slot
            }
        }
    }

}
