using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class CharacterInfoDisplay : MonoBehaviour
{
    public EquipmentDisplay equipmentDisplay; // Reference to the EquipmentDisplay script
    public TextMeshProUGUI infoText; // Displays character info
    public Image portraitImage; // Displays character portrait
    public Button Button_ChangeName;
    public TMP_InputField InputField_Name;
    public Button Button_SubmitName;
    public Button Button_ChangePortrait;

    private CharacterData currentCharacter;

    private void Start()
    {
        Button_ChangeName.onClick.AddListener(EnableNameChange);
        Button_SubmitName.onClick.AddListener(SubmitNewName);
        Button_ChangePortrait.onClick.AddListener(EnablePortraitChange);
        InputField_Name.gameObject.SetActive(false);
        Button_SubmitName.gameObject.SetActive(false);

        LoadSelectedCharacter();
    }

    void LoadSelectedCharacter()
    {
        if (CharacterManager.instance == null || GameManager.instance == null)
        {
            Debug.LogError("CharacterManager or GameManager instance is missing.");
            return;
        }

        int selectedNumber = GameManager.instance.selectedCharacterNumber;
        currentCharacter = CharacterManager.instance.GetCharacters().Find(c => c.characterNumber == selectedNumber);
        if (currentCharacter != null)
        {
            ShowCharacterInfo();
            InitializeEquipmentDisplay();
        }
        else
        {
            Debug.LogError($"Character with number {selectedNumber} not found!");
        }
    }

    void EnableNameChange()
    {
        InputField_Name.gameObject.SetActive(true);
        Button_SubmitName.gameObject.SetActive(true);
        InputField_Name.text = currentCharacter.characterName;
    }

    void SubmitNewName()
    {
        string newName = InputField_Name.text.Trim();
        if (!string.IsNullOrEmpty(newName))
        {
            currentCharacter.characterName = newName;
            GameManager.instance.SaveGame();
            ShowCharacterInfo();
        }
        InputField_Name.gameObject.SetActive(false);
        Button_SubmitName.gameObject.SetActive(false);
    }

    public void ShowCharacterInfo()
    {
        if (currentCharacter == null) return;

        infoText.text = $"Name: {currentCharacter.characterName}\n" +
                        $"Race: {currentCharacter.race}\n" +
                        $"Class: {currentCharacter.characterClass}\n" +
                        $"Level: {currentCharacter.level}\n\n" +
                        $"Equipment:\n" +
                        $"Weapon: {currentCharacter.weapon}\n" +
                        $"Head: {currentCharacter.head}\n" +
                        $"Top: {currentCharacter.top}\n" +
                        $"Bottom: {currentCharacter.bottom}\n" +
                        $"Shoes: {currentCharacter.shoes}\n" +
                        $"Ring: {currentCharacter.ring}\n" +
                        $"Necklace: {currentCharacter.necklace}\n" +
                        $"Bracelet: {currentCharacter.bracelet}\n\n";

        if (currentCharacter.portraitData != null)
        {
            portraitImage.sprite = ConvertByteArrayToSprite(currentCharacter.portraitData[0]);
        }
     }

    void EnablePortraitChange()
    {
        string filePath = UnityEditor.EditorUtility.OpenFilePanel("Select Portrait", "", "png,jpg,jpeg");

        if (!string.IsNullOrEmpty(filePath))
        {
            byte[] imageBytes = File.ReadAllBytes(filePath);
            currentCharacter.portraitData = new List<byte[]> { imageBytes };
            ShowCharacterInfo();
            GameManager.instance.SaveGame();
        }
    }

    void InitializeEquipmentDisplay()
    {
        if (equipmentDisplay != null)
        {
            // Pass the selected character to the EquipmentDisplay to initialize the slots
            equipmentDisplay.Initialize(currentCharacter);
        }
    }

    private Sprite ConvertByteArrayToSprite(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0) return null;
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        return null;
    }
}
