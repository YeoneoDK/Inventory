using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GuildInfoManager : MonoBehaviour
{
    public Button generateCharacterButton; // Button to generate a new character
    public Transform buttonParent; // Parent to hold dynamically created buttons
    public GameObject buttonPrefab; // Prefab of the button with portrait image

    public Image portraitImage;

    public Sprite[] humanPortraits;
    public Sprite[] elfPortraits;
    public Sprite[] orcPortraits;
    public Sprite[] undeadPortraits;
    public Sprite[] dragonbornPortraits;

    public int maxCharacterLimit = 16; // Set the maximum number of characters allowed

    private void Start()
    {
        generateCharacterButton.onClick.AddListener(GenerateNewCharacter);
        LoadExistingCharacters();
    }

    void LoadExistingCharacters()
    {
        if (GameManager.instance == null || GameManager.instance.gameData == null)
        {
            Debug.LogWarning("GameManager or GameData is null! GuildInfoManager cannot load characters.");
            return;
        }

        // Clear existing buttons before regenerating
        Debug.Log("Clearing existing character buttons...");
        foreach (Transform child in buttonParent.transform)
        {
            Destroy(child.gameObject);
        }

        List<CharacterData> characters = GameManager.instance.gameData.characters; // Get character list

        Debug.Log($"Loaded {characters.Count} characters.");

        if (characters.Count == 0)
        {
            Debug.LogWarning("No characters found after loading.");
            return;
        }

        // Generate a button for each character
        foreach (CharacterData character in characters)
        {
            Sprite characterPortrait = ConvertByteArrayToSprite(character.portraitData[0]);
            GenerateCharacterButton(character, characterPortrait);
        }
    }

    public void GenerateNewCharacter()
    {
        if (GameManager.instance.gameData.characters.Count >= maxCharacterLimit)
        {
            Debug.LogWarning("Character limit reached. Cannot generate more characters.");
            return; // Stop further execution if the limit is reached
        }

        int newCharacterNumber = GameManager.instance.gameData.characters.Count;
        Race randomRace = (Race)Random.Range(0, System.Enum.GetValues(typeof(Race)).Length);
        Class randomClass = (Class)Random.Range(0, System.Enum.GetValues(typeof(Class)).Length);

        CharacterData newCharacter = new CharacterData(newCharacterNumber, "Character" + newCharacterNumber, randomRace.ToString(), randomClass.ToString());
        Sprite selectedPortrait = GetRandomPortrait(randomRace);

        if (selectedPortrait == null)
        {
            Debug.LogError("No portrait available for race: " + randomRace);
            return;
        }

        newCharacter.portraitData = new List<byte[]> { ConvertSpriteToByteArray(selectedPortrait) };

        CharacterManager.instance.AddCharacter(newCharacter);
        GameManager.instance.SaveGame();

        GenerateCharacterButton(newCharacter, selectedPortrait);
    }

    void GenerateCharacterButton(CharacterData character, Sprite portrait)
    {
        GameObject newButton = Instantiate(buttonPrefab, buttonParent);
        Button buttonComponent = newButton.GetComponent<Button>();
        TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
        Image portraitImage = newButton.transform.Find("Image").GetComponent<Image>();

        if (buttonComponent == null || buttonText == null || portraitImage == null)
        {
            Debug.LogError("Button components missing for " + character.characterName);
            return;
        }

        buttonText.text = character.characterName;
        portraitImage.sprite = portrait;
        buttonComponent.onClick.AddListener(() => SelectCharacter(character.characterNumber));
    }

    void SelectCharacter(int characterNumber)
    {
        GameManager.instance.selectedCharacterNumber = characterNumber;
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterInfo");
    }

    private Sprite GetRandomPortrait(Race race)
    {
        switch (race)
        {
            case Race.Human:
                return humanPortraits.Length > 0 ? humanPortraits[Random.Range(0, humanPortraits.Length)] : null;
            case Race.Elf:
                return elfPortraits.Length > 0 ? elfPortraits[Random.Range(0, elfPortraits.Length)] : null;
            case Race.Orc:
                return orcPortraits.Length > 0 ? orcPortraits[Random.Range(0, orcPortraits.Length)] : null;
            case Race.Undead:
                return undeadPortraits.Length > 0 ? undeadPortraits[Random.Range(0, undeadPortraits.Length)] : null;
            case Race.Dragonborn:
                return dragonbornPortraits.Length > 0 ? dragonbornPortraits[Random.Range(0, dragonbornPortraits.Length)] : null;
            default:
                return null;
        }
    }

    private byte[] ConvertSpriteToByteArray(Sprite sprite)
    {
        if (sprite == null) return null;
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        texture.SetPixels(sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height));
        texture.Apply();
        byte[] imageData = texture.EncodeToPNG();
        Destroy(texture);
        return imageData;
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
