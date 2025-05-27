using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class CharacterCreation : MonoBehaviour
{
    public static CharacterManager instance;
    public TMP_InputField characterNameInputField; // Input field for character name    
    public Button[] raceButtons; // Buttons for selecting race (Human, Elf, Orc, Undead, Dragonborn)
    public Button[] classButtons; // Buttons for selecting class (Warrior, Ranger, Caster, Priest)
    public Button[] difficultyButtons; // Buttons for selecting difficulty (Easy, Normal, Hard)
    public Button[] gameModeButtons; // Buttons for selecting game mode (Softcore, Hardcore)
    public Button[] portraitButtons; // Buttons for selecting portraits
    public Button uploadPortraitButton; // Button to upload a custom portrait
    public Sprite[] portraits; // Array of 20 portrait sprites (make sure this is public)
    public Image largePortraitImage;
    public Button saveAndProceedButton; // Button to save all options and proceed

    private string selectedRace;
    private string selectedClass;
    private string selectedDifficulty;
    private string selectedGameMode;
    private Sprite selectedPortrait;

    // Call this method to save character data and proceed
    private void OnSaveAndProceed()
    {
        if (string.IsNullOrEmpty(characterNameInputField.text))
        {
            Debug.LogError("Character name cannot be empty");
            return;
        }

        // Check if portrait is selected
        if (selectedPortrait == null)
        {
            Debug.LogError("No portrait selected");
            return;
        }

        // Create character data
        CharacterData newCharacter = new CharacterData
        (
            0,
            characterNameInputField.text, // Pass the name from the input field
            selectedRace,                 // Selected race
            selectedClass                 // Selected class
        );

        if (selectedPortrait != null)
        {
            newCharacter.portraitData.Add(ImageToByteArray(selectedPortrait));
        }

        // Save character data to CharacterManager
        Debug.Log("Character created: " + newCharacter.characterName);
        CharacterManager.instance.AddCharacter(newCharacter);

        // Save difficulty and game mode to GameManager
        GameManager.instance.SetDifficulty(selectedDifficulty);
        GameManager.instance.SetGameMode(selectedGameMode);

        newCharacter.top = 0; 
        newCharacter.head = 3;
        GameManager.instance.AddDefaultItemsToVault(); // Ensure default vault items are added


        GameSaveManager saveManager = GameManager.instance.GetComponent<GameSaveManager>();
        if (saveManager != null)
        {
            saveManager.AutoSaveGame(characterNameInputField.text); // Auto-save using the character's name
        }
        // Proceed to the next scene (change "Central" to your scene name)
        SceneManager.LoadScene("Central");
    }

    private void Start()
    {
        // Set default selections
        selectedRace = "Human";
        selectedClass = "Warrior";
        selectedPortrait = null;  // First portrait (index 0)
        selectedDifficulty = "Normal";
        selectedGameMode = "Softcore";

        // Add listeners to buttons
        foreach (Button btn in raceButtons)
        {
            btn.onClick.AddListener(() => OnRaceSelected(btn));
        }

        foreach (Button btn in classButtons)
        {
            btn.onClick.AddListener(() => OnClassSelected(btn));
        }

        foreach (Button btn in difficultyButtons)
        {
            btn.onClick.AddListener(() => OnDifficultySelected(btn));
        }

        foreach (Button btn in gameModeButtons)
        {
            btn.onClick.AddListener(() => OnGameModeSelected(btn));
        }

        foreach (Button btn in portraitButtons)
        {
            btn.onClick.AddListener(() => OnPortraitSelected(btn.GetComponent<Image>().sprite));
        }

        // Initially, hide all portrait buttons
        foreach (Button btn in portraitButtons)
        {
            btn.gameObject.SetActive(false);
        }

        if (uploadPortraitButton != null)
        {
            uploadPortraitButton.onClick.AddListener(OnUploadPortraitButtonClicked);
        }

        if (saveAndProceedButton != null)
        {
            saveAndProceedButton.onClick.AddListener(OnSaveAndProceed);
        }
    }

    private void OnUploadPortraitButtonClicked()
    {
        // Open file picker and allow the user to choose an image file (e.g., PNG, JPG)
        string path = EditorUtility.OpenFilePanel("Select Portrait Image", "", "jpg,png");

        if (!string.IsNullOrEmpty(path))
        {
            // Load the image from the path as a Texture2D
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // This will automatically resize the texture

            // Resize the texture to 300x300
            Texture2D resizedTexture = ResizeTexture(texture, 300, 300);

            // Convert the resized texture to a Sprite
            selectedPortrait = Sprite.Create(resizedTexture, new Rect(0, 0, resizedTexture.width, resizedTexture.height), new Vector2(0.5f, 0.5f));

            // Display the larger portrait image
            if (largePortraitImage != null)
            {
                largePortraitImage.sprite = selectedPortrait;
                largePortraitImage.gameObject.SetActive(true); // Show the larger image
            }

            SaveTextureAsPNG(resizedTexture, "Portrait_300x300.png");

            Debug.Log("Custom Portrait uploaded and saved successfully.");
        }
    }

    private Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
    {
        Texture2D resizedTexture = new Texture2D(width, height, originalTexture.format, false);
        Color[] pixels = originalTexture.GetPixels(0, 0, originalTexture.width, originalTexture.height);

        resizedTexture.SetPixels(0, 0, width, height, pixels);
        resizedTexture.Apply();
        return resizedTexture;
    }

    // Save the texture to a file (e.g., PNG)
    private void SaveTextureAsPNG(Texture2D texture, string fileName)
    {
        byte[] pngData = texture.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, fileName); // Save in persistent data path
        File.WriteAllBytes(path, pngData);

        Debug.Log($"Portrait saved to: {path}");
    }

    // Handle race selection
    private void OnRaceSelected(Button btn)
    {
        if (btn.name.Contains("Human"))
            selectedRace = "Human";
        else if (btn.name.Contains("Elf"))
            selectedRace = "Elf";
        else if (btn.name.Contains("Orc"))
            selectedRace = "Orc";
        else if (btn.name.Contains("Undead"))
            selectedRace = "Undead";
        else if (btn.name.Contains("Dragonborn"))
            selectedRace = "Dragonborn";

        Debug.Log("Selected Race: " + selectedRace);
        UpdatePortraitsBasedOnRace();
    }

    // Handle class selection
    private void OnClassSelected(Button btn)
    {
        if (btn.name.Contains("Warrior"))
            selectedClass = "Warrior";
        else if (btn.name.Contains("Ranger"))
            selectedClass = "Ranger";
        else if (btn.name.Contains("Caster"))
            selectedClass = "Caster";
        else if (btn.name.Contains("Priest"))
            selectedClass = "Priest";

        Debug.Log("Selected Class: " + selectedClass);
    }

    // Handle difficulty selection
    private void OnDifficultySelected(Button btn)
    {
        if (difficultyButtons == null || difficultyButtons.Length == 0)
        {
            Debug.LogError("Difficulty buttons are not assigned in the Inspector!");
            return;
        }

        if (btn.name.Contains("Easy"))
            selectedDifficulty = "Easy";
        else if (btn.name.Contains("Normal"))
            selectedDifficulty = "Normal";
        else if (btn.name.Contains("Hard"))
            selectedDifficulty = "Hard";

        // Ensure GameManager is not null before calling SetDifficulty
        if (GameManager.instance != null)
        {
            GameManager.instance.SetDifficulty(selectedDifficulty); // Set in GameManager
        }
        else
        {
            Debug.LogError("GameManager instance is not assigned!");
        }
    }

    private void OnGameModeSelected(Button btn)
    {
        if (gameModeButtons == null || gameModeButtons.Length == 0)
        {
            Debug.LogError("Game mode buttons are not assigned in the Inspector!");
            return;
        }

        if (btn.name.Contains("Softcore"))
            selectedGameMode = "Softcore";
        else if (btn.name.Contains("Hardcore"))
            selectedGameMode = "Hardcore";

        // Ensure GameManager is not null before calling SetGameMode
        if (GameManager.instance != null)
        {
            GameManager.instance.SetGameMode(selectedGameMode); // Set in GameManager
        }
        else
        {
            Debug.LogError("GameManager instance is not assigned!");
        }
    }

    // Handle portrait selection
    private void OnPortraitSelected(Sprite selectedPortrait)
    {
        this.selectedPortrait = selectedPortrait;
        // Set the larger portrait display
        if (largePortraitImage != null)
        {
            largePortraitImage.sprite = selectedPortrait;
            largePortraitImage.gameObject.SetActive(true); // Show the larger image
        }

        if (uploadPortraitButton.gameObject.activeSelf == false)
        {
            // Resize the selected portrait and save it
            Texture2D resizedTexture = ResizeTexture(SpriteToTexture(selectedPortrait), 300, 300);
            SaveTextureAsPNG(resizedTexture, selectedRace + "_" + selectedClass + "_Portrait_300x300.png");
        }
    }

    private Texture2D SpriteToTexture(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        Texture2D textureCopy = new Texture2D(texture.width, texture.height);
        textureCopy.SetPixels(texture.GetPixels());
        textureCopy.Apply();
        return textureCopy;
    }

    // Update portrait buttons based on selected race
    private void UpdatePortraitsBasedOnRace()
    {
        // Hide all portrait buttons initially
        foreach (Button btn in portraitButtons)
        {
            btn.gameObject.SetActive(false);
        }

        // Based on selected race, show the appropriate set of portrait buttons
        switch (selectedRace)
        {
            case "Human":
                ShowPortraits(0, 4); // Human portraits, adjust indices as needed
                break;
            case "Elf":
                ShowPortraits(4, 8); // Elf portraits, adjust indices as needed
                break;
            case "Orc":
                ShowPortraits(8, 12); // Orc portraits, adjust indices as needed
                break;
            case "Undead":
                ShowPortraits(12, 16); // Undead portraits, adjust indices as needed
                break;
            case "Dragonborn":
                ShowPortraits(16, 20); // Dragonborn portraits, adjust indices as needed
                break;
        }
    }

    // Helper method to show race-based portrait buttons
    private void ShowPortraits(int startIndex, int endIndex)
    {
        // Ensure that the end index does not exceed the size of the portraits array
        endIndex = Mathf.Min(endIndex, portraits.Length);

        // Enable the portrait buttons and assign the sprites
        for (int i = 0; i < portraitButtons.Length; i++)
        {
            if (i < (endIndex - startIndex))
            {
                portraitButtons[i].gameObject.SetActive(true); // Enable the button
                Image buttonImage = portraitButtons[i].GetComponent<Image>(); // Get the button's Image component
                buttonImage.sprite = portraits[startIndex + i]; // Assign the corresponding portrait sprite
            }
            else
            {
                portraitButtons[i].gameObject.SetActive(false); // Disable buttons if there are fewer portraits
            }
        }
    }

    private byte[] ImageToByteArray(Sprite sprite)
    {
        Texture2D texture = SpriteToTexture(sprite);
        return texture.EncodeToPNG();
    }
}
