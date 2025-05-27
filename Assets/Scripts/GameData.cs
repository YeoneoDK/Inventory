using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int stage;
    public string gameMode;
    public string difficulty;

    // Character data
    public List<CharacterData> characters = new List<CharacterData>();

    // Vault data
    public List<int> equipmentVaultCodes = new List<int>();
    public List<int> skillgemVault = new List<int>();
}

[System.Serializable]
public class CharacterData
{
    public int characterNumber;
    public string characterName;
    public string race;
    public string characterClass;
    public int level;
    public int weapon;
    public int head;
    public int top;
    public int bottom;
    public int shoes;
    public int ring;
    public int necklace;
    public int bracelet;
    public int skillgem1;  // Stores skillgem codes
    public int skillgem2;
    public int skillgem3;
    public int skillgem4;
    public int skillgem5;
    public List<byte[]> portraitData;  // Assuming you want to store portrait data as byte arrays

    // Constructor with parameters for characterNumber, characterName, race, and characterClass
    public CharacterData(int characterNumber, string characterName, string race, string characterClass)
    {
        this.characterNumber = characterNumber;
        this.characterName = characterName;
        this.race = race;
        this.characterClass = characterClass;
        this.level = 1;  // Default level can be set here
        this.weapon = -1; // Default values for other fields
        this.head = -1;
        this.top = -1;
        this.bottom = -1;
        this.shoes = -1;
        this.ring = -1;
        this.necklace = -1;
        this.bracelet = -1;
        this.skillgem1 = -1;
        this.skillgem2 = -1;
        this.skillgem3 = -1;
        this.skillgem4 = -1;
        this.skillgem5 = -1;
        this.portraitData = new List<byte[]>(); // Initialize portrait data
    }
    public int GetItemCode(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon: return weapon;
            case EquipmentSlot.Head: return head;
            case EquipmentSlot.Top: return top;
            case EquipmentSlot.Bottom: return bottom;
            case EquipmentSlot.Shoes: return shoes;
            case EquipmentSlot.Ring: return ring;
            case EquipmentSlot.Necklace: return necklace;
            case EquipmentSlot.Bracelet: return bracelet;
            default: return -1;  // Return -1 if no valid slot is provided
        }
    }
}

[System.Serializable]

public enum Race
{
    Human,
    Elf,
    Orc,
    Undead,
    Dragonborn
}

public enum Class
{ 
    Warrior,
    Ranger,
    Caster,
    Priest
}

