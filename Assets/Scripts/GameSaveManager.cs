using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using TMPro;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager instance;

    private string saveFilePath;
    public GameObject saveMenuUI;
    public GameObject loadMenuUI;
    public GameObject pauseMenuUI;
    public TMP_InputField inputField_Name;
    public GameData currentGameData;
    public int maxSaveFiles = 7;
    public LoadSaveFiles loadSaveFilesUI; // Reference to the LoadSaveFiles script


    GameObject FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child.gameObject;  // Found it!
            GameObject result = FindDeepChild(child, name);   // Recursively check child objects
            if (result != null) return result;
        }
        return null;
    }

    void ReassignUIReferences()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas NOT found!");
            return;
        }
        inputField_Name = FindDeepChild(saveMenuUI.transform, "InputField_Name")?.GetComponent<TMP_InputField>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene changed to: " + scene.name);

        pauseMenuUI = GameObject.Find("Canvas")?.transform.Find("Panel_PauseMenu")?.gameObject;
        saveMenuUI = GameObject.Find("Canvas")?.transform.Find("Panel_SaveMenu")?.gameObject;
        loadMenuUI = GameObject.Find("Canvas")?.transform.Find("Panel_LoadMenu")?.gameObject;
        ReassignUIReferences();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;  // Register event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        Debug.Log("GameSaveManager is in the scene and initialized.");
        saveFilePath = Application.persistentDataPath + "/SaveFiles/";
        Debug.Log("Save files are stored at: " + saveFilePath);
    }

    // Method to manually save game from Pause Menu
    public void SaveGame()
    {
        string saveFileName = inputField_Name.text;

        if (string.IsNullOrEmpty(saveFileName))
        {
            Debug.Log("Please enter a valid save file name.");
            return;
        }

        if (!Directory.Exists(saveFilePath))
        {
            Directory.CreateDirectory(saveFilePath);
        }

        string[] files = Directory.GetFiles(saveFilePath, "*.json");
        if (files.Length >= maxSaveFiles)
        {
            Debug.Log("Maximum number of save files reached.");
            return;
        }

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is not initialized.");
            return;
        }

        currentGameData = GameManager.instance.GetGameData();

        if (currentGameData == null)
        {
            Debug.LogError("currentGameData is null. Make sure GameManager has valid data.");
            return;
        }

        string jsonData = JsonConvert.SerializeObject(currentGameData, Formatting.Indented);
        string filePath = saveFilePath + saveFileName + ".json";

        File.WriteAllText(filePath, jsonData);

        // Refresh the save list after saving
        if (loadSaveFilesUI != null)
        {
            loadSaveFilesUI.LoadSaveFilesFromDirectory();
        }

        Debug.Log("Game saved to: " + filePath);
        if (saveMenuUI != null) saveMenuUI.SetActive(false);
    }

    // Method to automatically save game when character is created (Character Creation Scene)
    public void AutoSaveGame(string characterName)
    {
        // Ensure save directory exists
        if (!Directory.Exists(saveFilePath))
        {
            Directory.CreateDirectory(saveFilePath);
        }

        string[] files = Directory.GetFiles(saveFilePath, "*.json");
        if (files.Length >= maxSaveFiles)
        {
            Debug.Log("Maximum number of save files reached.");
            return;
        }

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is not initialized.");
            return;
        }

        currentGameData = GameManager.instance.GetGameData();

        if (currentGameData == null)
        {
            Debug.LogError("currentGameData is null. Make sure GameManager has valid data.");
            return;
        }

        // Using character name as the save file name
        string jsonData = JsonConvert.SerializeObject(currentGameData, Formatting.Indented);
        string filePath = saveFilePath + characterName + "_autoSave.json"; // Auto save file named by character name

        File.WriteAllText(filePath, jsonData);

        Debug.Log("Game auto-saved to: " + filePath);
    }

    // Method to load a game from a save file
    public GameData LoadGame(string saveFileName)
    {
        string filePath = saveFilePath + saveFileName + ".json";

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            currentGameData = JsonConvert.DeserializeObject<GameData>(jsonData);
            GameManager.instance.ApplyLoadedGameData(currentGameData);
            return currentGameData;
        }
        else
        {
            Debug.LogWarning("Save file not found: " + filePath);
            return null;
        }
    }

    // Menu open/close methods
    public void OpenSaveMenu()
    {
        LoadSaveFiles loadSaveFiles = FindFirstObjectByType<LoadSaveFiles>();
        if (loadSaveFilesUI != null)
        {
            loadSaveFilesUI.LoadSaveFilesFromDirectory();
        }
        saveMenuUI.SetActive(true);
        AssignButtonEvents();
    }

    public void OpenLoadMenu()
    {
        if (loadSaveFilesUI != null)
        {
            loadSaveFilesUI.LoadSaveFilesFromDirectory();
        }
        loadMenuUI.SetActive(true);
        AssignButtonEvents();
        LoadSaveFiles.instance.AssignContainer();
    }

    public void CloseMenus()
    {
        saveMenuUI.SetActive(false);
        loadMenuUI.SetActive(false);
    }

    void AssignButtonEvents()
    {
        Button saveButton = FindDeepChild(saveMenuUI.transform, "Button_Save")?.GetComponent<Button>();
        Button cancelButton = FindDeepChild(saveMenuUI.transform, "Button_Cancel")?.GetComponent<Button>();
        Button closeButton = FindDeepChild(loadMenuUI.transform, "Button_Close")?.GetComponent<Button>();

        Debug.Log("Save Button: " + (saveButton != null ? "Found" : "Not Found"));
        Debug.Log("Cancel Button: " + (cancelButton != null ? "Found" : "Not Found"));
        Debug.Log("Close Button: " + (closeButton != null ? "Found" : "Not Found"));

        if (saveButton != null)
        {
            saveButton.onClick.RemoveAllListeners(); // Clear old listeners
            saveButton.onClick.AddListener(SaveGame);
        }
        else Debug.LogError("Save Button not found!");

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CloseMenus);
        }
        else Debug.LogError("Load Button not found!");

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseMenus);
        }
        else Debug.LogError("Resume Button not found!");
    }
}
