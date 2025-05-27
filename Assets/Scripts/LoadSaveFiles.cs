using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using System.IO;
using UnityEngine.UI;  // For Button usage

public class LoadSaveFiles : MonoBehaviour
{
    public Transform contentTransform;  // Parent container for save files
    public GameObject saveFileItemPrefab;  // Prefab for displaying save files
    private string saveFilePath;
    public GameSaveManager gameSaveManager;
    public static LoadSaveFiles instance;  // Singleton instance
   
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Make sure this object persists across scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
    }

    public void AssignContainer()
    {
         // Find the parent container (Canvas) first
        Transform parentContainer = GameObject.Find("Canvas/Panel_LoadMenu")?.transform;

        if (parentContainer != null)
        {
            // Then search for the SaveFileListContainer under that parent
            Transform saveFileListContainer = parentContainer.Find("SaveFileListContainer");

            if (saveFileListContainer != null)
            {
                LoadSaveFiles.instance.SetContentTransform(saveFileListContainer);  // Set the reference in the LoadSaveFiles script
            }
            else
            {
                Debug.LogError("SaveFileListContainer not found under Panel_LoadM!");
            }
        }
        else
        {
            Debug.LogError("Panel_LoadMenu Files not found in the scene!");
        }
    }

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/SaveFiles/";
        LoadSaveFilesFromDirectory();
    }

    public void LoadSaveFilesFromDirectory()
    {
        // Clear old UI elements
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // Ensure save directory exists
        if (!Directory.Exists(saveFilePath))
        {
            Directory.CreateDirectory(saveFilePath);
        }

        string[] files = Directory.GetFiles(saveFilePath, "*.json");
        int maxSaveFiles = 7;
        int fileCount = 0;

        foreach (string file in files)
        {
            if (fileCount >= maxSaveFiles) break;

            string fileName = Path.GetFileNameWithoutExtension(file);
            string saveDate = File.GetLastWriteTime(file).ToString("g");

            GameObject saveFileItem = Instantiate(saveFileItemPrefab, contentTransform);
            TextMeshProUGUI[] textFields = saveFileItem.GetComponentsInChildren<TextMeshProUGUI>();
            textFields[0].text = fileName;
            textFields[1].text = saveDate;

            Button loadButton = saveFileItem.GetComponentInChildren<Button>();
            Button deleteButton = saveFileItem.transform.Find("Button_Delete").GetComponent<Button>();

            loadButton.onClick.AddListener(() => OnSaveFileSelected(fileName));
            deleteButton.onClick.AddListener(() => OnDeleteFile(fileName, saveFileItem));

            fileCount++;
        }
    }

    public void SetContentTransform(Transform newContentTransform)
    {
        contentTransform = newContentTransform;
        LoadSaveFilesFromDirectory();  // Refresh the save files UI
    }

    void OnSaveFileSelected(string saveFileName)
    {
        Debug.Log("Loading save file: " + saveFileName);

        if (gameSaveManager != null)
        {
            gameSaveManager.LoadGame(saveFileName);
            gameSaveManager.loadMenuUI.SetActive(false);
        }
        else
        {
            Debug.LogError("GameSaveManager is not assigned in LoadSaveFiles script.");
        }
    }

    void OnDeleteFile(string saveFileName, GameObject saveFileItem)
    {
        string filePath = saveFilePath + saveFileName + ".json";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Destroy(saveFileItem);
            LoadSaveFilesFromDirectory();
        }
        else
        {
            Debug.LogWarning("Save file not found: " + saveFileName);
        }
    }
}
