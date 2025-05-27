using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquipmentDisplay : MonoBehaviour
{
    public CharacterData currentCharacter;
    public Dictionary<EquipmentSlot, Image> slotImages = new Dictionary<EquipmentSlot, Image>();

    public Image weaponSlotImage; // Image component for the Weapon slot
    public Image headSlotImage; // Image component for the Head slot
    public Image topSlotImage; // Image component for the Top slot
    public Image bottomSlotImage; // Image component for the Bottom slot
    public Image shoesSlotImage; // Image component for the Shoes slot
    public Image ringSlotImage; // Image component for the Ring slot
    public Image necklaceSlotImage; // Image component for the Necklace slot
    public Image braceletSlotImage; // Image component for the Bracelet slot

    public void Initialize(CharacterData character)
    {
        currentCharacter = character;
        Debug.Log("Initializing equipment for character: " + currentCharacter.characterName);
        RegisterSlot(EquipmentSlot.Weapon, weaponSlotImage);
        RegisterSlot(EquipmentSlot.Head, headSlotImage);
        RegisterSlot(EquipmentSlot.Top, topSlotImage);
        RegisterSlot(EquipmentSlot.Bottom, bottomSlotImage);
        RegisterSlot(EquipmentSlot.Shoes, shoesSlotImage);
        RegisterSlot(EquipmentSlot.Ring, ringSlotImage);
        RegisterSlot(EquipmentSlot.Necklace, necklaceSlotImage);
        RegisterSlot(EquipmentSlot.Bracelet, braceletSlotImage);
        UpdateEquipmentDisplay();
    }

    public void RegisterSlot(EquipmentSlot slot, Image slotImage)
    {
        slotImages[slot] = slotImage;
    }

    public void UpdateEquipmentDisplay()
    {
        if (currentCharacter == null) return;
        Debug.Log("UpdateEquipmentDisplay called for character: " + currentCharacter.characterName);
        foreach (var slot in slotImages.Keys)
        {
            int itemCode = GetItemCodeFromCharacter(slot);
            EquipmentItem item = ItemDatabase.GetItemByCode(itemCode);

            if (item != null)
            {
                Debug.Log($"Slot: {slot} - Item: {item.itemName}, Icon: {item.icon}");
                if (item.icon != null)
                {
                    slotImages[slot].sprite = item.icon;
                    slotImages[slot].color = Color.white; // Make visible
                }
                else
                {
                    slotImages[slot].sprite = null;
                    slotImages[slot].color = new Color(1, 1, 1, 0); // Hide if no icon
                }
            }
            else
            {
                slotImages[slot].sprite = null;
                slotImages[slot].color = new Color(1, 1, 1, 0); // Hide if no item
            }

            // Refresh the UI
            slotImages[slot].SetNativeSize();
            slotImages[slot].rectTransform.sizeDelta = new Vector2(100, 100); // Enforce size
        }
    }

    public void UpdateEquipmentSlot(EquipmentSlot slot, EquipmentItem item)
    {
        int code = item != null ? item.codeNumber : -1;

        switch (slot)
        {
            case EquipmentSlot.Weapon: currentCharacter.weapon = code; break;
            case EquipmentSlot.Head: currentCharacter.head = code; break;
            case EquipmentSlot.Top: currentCharacter.top = code; break;
            case EquipmentSlot.Bottom: currentCharacter.bottom = code; break;
            case EquipmentSlot.Shoes: currentCharacter.shoes = code; break;
            case EquipmentSlot.Ring: currentCharacter.ring = code; break;
            case EquipmentSlot.Necklace: currentCharacter.necklace = code; break;
            case EquipmentSlot.Bracelet: currentCharacter.bracelet = code; break;
        }

        UpdateEquipmentDisplay();
    }


    private int GetItemCodeFromCharacter(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon: return currentCharacter.weapon;
            case EquipmentSlot.Head: return currentCharacter.head;
            case EquipmentSlot.Top: return currentCharacter.top;
            case EquipmentSlot.Bottom: return currentCharacter.bottom;
            case EquipmentSlot.Shoes: return currentCharacter.shoes;
            case EquipmentSlot.Ring: return currentCharacter.ring;
            case EquipmentSlot.Necklace: return currentCharacter.necklace;
            case EquipmentSlot.Bracelet: return currentCharacter.bracelet;
            default: return -1;
        }
    }


}