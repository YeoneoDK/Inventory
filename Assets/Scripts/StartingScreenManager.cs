using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;  // JSON Handling

public class StartingScreenManager : MonoBehaviour
{
    public Button loadButton;
    public Button newGameButton;
    public Button optionsButton;
    public Button exitButton;

    private string saveFilePath;

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/SaveFiles/";

        loadButton.onClick.AddListener(LoadMostRecentSave);
        newGameButton.onClick.AddListener(StartNewGame);
        optionsButton.onClick.AddListener(OpenOptions);
        exitButton.onClick.AddListener(ExitGame);
    }

    void LoadMostRecentSave()
    {
        if (!Directory.Exists(saveFilePath))
        {
            Debug.Log("No save folder found!");
            return;
        }

        string[] files = Directory.GetFiles(saveFilePath, "*.json");

        if (files.Length == 0)
        {
            Debug.Log("No save files found!");
            return;
        }

        string mostRecentFile = "";
        System.DateTime mostRecentTime = System.DateTime.MinValue;

        foreach (string file in files)
        {
            System.DateTime lastWriteTime = File.GetLastWriteTime(file);
            if (lastWriteTime > mostRecentTime)
            {
                mostRecentTime = lastWriteTime;
                mostRecentFile = file;
            }
        }

        if (!string.IsNullOrEmpty(mostRecentFile))
        {
            string saveFileName = Path.GetFileNameWithoutExtension(mostRecentFile);
            Debug.Log("Loading most recent save: " + saveFileName);

            if (GameSaveManager.instance != null)
            {
                GameSaveManager.instance.LoadGame(saveFileName);
                SceneManager.LoadScene("Central");
            }
            else
            {
                Debug.LogError("GameSaveManager instance is null. Ensure it exists in the scene.");
            }
        }
    }

    void StartNewGame()
    {
        SceneManager.LoadScene("CharacterCreation");
    }

    void OpenOptions()
    {
        Debug.Log("Opening Options Menu...");
    }

    void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
