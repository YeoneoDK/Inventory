using UnityEngine;
using System.Collections.Generic;

public static class ItemDatabase
{
    private static Dictionary<int, EquipmentItem> itemDictionary = new Dictionary<int, EquipmentItem>();
    private static Dictionary<int, Sprite> iconDictionary = new Dictionary<int, Sprite>();
    private static bool initialized = false;

    // Initialize the database with items
    public static void Initialize()
    {
        if (initialized) return; // Ensure it runs only once
        initialized = true;
        Debug.Log("Initializing Item Database...");
        itemDictionary.Clear();
        iconDictionary.Clear();

        LoadIcons();  // Load the icons from Resources

        // Adding some predefined items (e.g., armor, weapons)
        AddItem(new EquipmentItem(0, "커먼상의", EquipmentSlot.Top, ItemRarity.Common, 1, "", 0, 1));
        AddItem(new EquipmentItem(1, "레어상의", EquipmentSlot.Top, ItemRarity.Rare, 1, "", 0, 2));
        AddItem(new EquipmentItem(2, "유니크상의", EquipmentSlot.Top, ItemRarity.Unique, 1, "", 0, 3));
        AddItem(new EquipmentItem(3, "커먼투구", EquipmentSlot.Head, ItemRarity.Common, 1, "", 0, 1));

        Debug.Log("Item Database Initialized.");
    }

    // Load icons from Resources folder
    private static void LoadIcons()
    {
        Sprite[] loadedIcons = Resources.LoadAll<Sprite>("Sprites/Items");
        Debug.Log($"Loaded {loadedIcons.Length} icons");
        foreach (Sprite icon in loadedIcons)
        {
            Debug.Log($"Loaded icon: {icon.name}");
            if (int.TryParse(icon.name, out int code)) // Ensure icon name matches code number
            {
                iconDictionary[code] = icon;
                Debug.Log($"Loaded icon for item code {code}: {icon.name}");
            }
            else
            {
                Debug.LogWarning($"Skipping invalid icon name: {icon.name}");
            }
        }
    }

    // Add a new item to the database
    public static void AddItem(EquipmentItem item)
    {
        if (itemDictionary.ContainsKey(item.codeNumber))
        {
            Debug.LogWarning($"Item with code {item.codeNumber} already exists!");
            return;
        }

        itemDictionary[item.codeNumber] = item;

        // Assign icon if available
        if (iconDictionary.TryGetValue(item.codeNumber, out Sprite sprite))
        {
            item.SetIcon(sprite);
        }
        else
        {
            Debug.LogWarning($"No icon found for item {item.itemName} (Code {item.codeNumber})");
        }

        Debug.Log($"Added new item: {item.itemName}");
    }

    // Get an item by its code number
    public static EquipmentItem GetItemByCode(int codeNumber)
    {
        return itemDictionary.TryGetValue(codeNumber, out EquipmentItem item) ? item : null;
    }
}
