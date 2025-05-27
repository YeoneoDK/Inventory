using UnityEngine;

public class EquipmentItem
{
    public int codeNumber; // Unique code for the item
    public string itemName; // Name of the item
    public EquipmentSlot slot; // The equipment slot (Weapon, Head, Top, etc.)
    public ItemRarity rarity; // The rarity (Common, Rare, Epic, Legendary)
    public int level; // The required level to equip this item
    public string itemType; // Only used for weapons (e.g., "Sword", "Bow", "Staff", etc.)
    public int attack; // Damage value (for weapons)
    public int defense; // Defense value (for armor)

    public Sprite icon; // Icon representing the item

    // Constructor for creating an item
    public EquipmentItem(int codeNumber, string itemName, EquipmentSlot slot, ItemRarity rarity, int level, string itemType, int attack, int defense)
    {
        this.codeNumber = codeNumber;
        this.itemName = itemName;
        this.slot = slot;
        this.rarity = rarity;
        this.level = level;
        this.itemType = slot == EquipmentSlot.Weapon ? itemType : ""; // Only assign itemType for weapons
        this.attack = attack;
        this.defense = defense;
    }

    // Set icon for the item (from icon database)
    public void SetIcon(Sprite sprite)
    {

        if (sprite != null)
        {
            icon = sprite;
            Debug.Log($"Icon set for item {itemName}: {sprite.name}");
        }
        else
        {
            Debug.LogWarning($"No icon assigned to {itemName}");
        }
    }
}

public enum EquipmentSlot
{
    Weapon,
    Head,
    Top,
    Bottom,
    Shoes,
    Ring,
    Necklace,
    Bracelet
}

public enum ItemRarity
{
    Common,
    Rare,
    Unique,
    Epic,
    Legendary
}

public enum ItemType
{
    Sword,
    Bow,
    Wand,
    Staff
}
