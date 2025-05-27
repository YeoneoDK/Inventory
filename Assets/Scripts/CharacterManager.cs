using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("CharacterManager instance initialized.");
        }
        else
        {
            Debug.Log("CharacterManager instance already exists. Destroying this one.");
            Destroy(gameObject);
        }
    }

    // Add character and auto-save after adding (only for CharacterCreation scene)
    public void AddCharacter(CharacterData character)
    {
        if (instance == null)
        {
            Debug.LogError("CharacterManager instance is null");
            return;
        }
        GameManager.instance.gameData.characters.Add(character);

        // Auto-save only for CharacterCreation scene
        if (SceneManager.GetActiveScene().name == "CharacterCreation")
        {
            GameSaveManager.instance.AutoSaveGame(character.characterName); // Auto-save after adding character
        }
    }

    public List<CharacterData> GetCharacters()
    {
        return GameManager.instance.gameData.characters;
    }

    public CharacterData GetSelectedCharacter()
    {
        if (GameManager.instance == null || CharacterManager.instance == null)
        {
            Debug.LogError("GameManager or CharacterManager instance is missing.");
            return null;
        }

        int selectedNumber = GameManager.instance.selectedCharacterNumber;
        return CharacterManager.instance.GetCharacters().Find(c => c.characterNumber == selectedNumber);
    }

    private void EquipItem(CharacterData character, EquipmentSlot slot, int itemCode)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon: character.weapon = itemCode; break;
            case EquipmentSlot.Head: character.head = itemCode; break;
            case EquipmentSlot.Top: character.top = itemCode; break;
            case EquipmentSlot.Bottom: character.bottom = itemCode; break;
            case EquipmentSlot.Shoes: character.shoes = itemCode; break;
            case EquipmentSlot.Ring: character.ring = itemCode; break;
            case EquipmentSlot.Necklace: character.necklace = itemCode; break;
            case EquipmentSlot.Bracelet: character.bracelet = itemCode; break;
        }
    }

    private void UnequipItem(int itemCode)
    {
        CharacterData character = GetSelectedCharacter();
        if (character == null) return;

        if (character.weapon == itemCode) character.weapon = -1;
        if (character.head == itemCode) character.head = -1;
        if (character.top == itemCode) character.top = -1;
        if (character.bottom == itemCode) character.bottom = -1;
        if (character.shoes == itemCode) character.shoes = -1;
        if (character.ring == itemCode) character.ring = -1;
        if (character.necklace == itemCode) character.necklace = -1;
        if (character.bracelet == itemCode) character.bracelet = -1;
    }

}