using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour
{
    public static PauseMenuHandler instance;  // Singleton instance
    public GameObject pauseMenuUI;  // Reference to pause menu UI
    public GameObject backgroundImage;
    public GameObject saveMenuUI;  // Reference to save menu UI
    public GameObject loadMenuUI;  // Reference to load menu UI
    public GameSaveManager saveManager;  // Reference to the GameSaveManager

    private bool isPaused = false;

    private string[] excludedScenes = { "CharacterCreation","StartingScreen" };

    bool ArrayContains(string value, string[] array)
    {
        foreach (string scene in array)
        {
            if (scene == value)
            {
                return true;
            }
        }
        return false;
    }

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
        backgroundImage = FindDeepChild(canvas.transform, "Image_Dark");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene changed to: " + scene.name);

        pauseMenuUI = GameObject.Find("Canvas")?.transform.Find("Panel_PauseMenu")?.gameObject;
        saveMenuUI = GameObject.Find("Canvas")?.transform.Find("Panel_SaveMenu")?.gameObject;
        loadMenuUI = GameObject.Find("Canvas")?.transform.Find("Panel_LoadMenu")?.gameObject;
        ReassignUIReferences();
        AssignButtonEvents();
    }

    void Start()
    {
        if (pauseMenuUI == null)
        {
            Debug.LogError("pauseMenuUI is missing in this scene.");
        }
   }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Make sure this object persists across scenes
            SceneManager.sceneLoaded += OnSceneLoaded;  // Register event
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
     }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (ArrayContains(SceneManager.GetActiveScene().name, excludedScenes))
        {
            return;  // Don't trigger pause menu in these scenes
        }
        // Toggle the pause menu when the ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            TogglePauseMenu(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            TogglePauseMenu(false);
        }
    }

    public void TogglePauseMenu(bool pause)
    {
        isPaused = pause;

        if (isPaused)
        {
            pauseMenuUI.SetActive(true);  // Show pause menu
            backgroundImage.SetActive(true);
            saveManager.CloseMenus();  // Ensure save/load menus are hidden
        }
        else
        {
            pauseMenuUI.SetActive(false);  // Hide pause menu
            backgroundImage.SetActive(false);
            saveManager.CloseMenus();  // Ensure save/load menus are hidden
        }
    }

    public void SaveButtonClicked()
    {
        // Open the Save Menu and let the user define the file name
        saveManager.OpenSaveMenu();

    }

    public void LoadButtonClicked()
    {
        // Open the Load Menu to allow the user to choose a file to load
        saveManager.OpenLoadMenu();
    }

    public void ResumeButtonClicked()
    {
        TogglePauseMenu(false);
        Time.timeScale = 1;  // Resume the game
    }
    
    public void mainMenuButtonClicked()
    {
        SceneManager.LoadScene("StartingScreen");
    }

    void AssignButtonEvents()
    {
        Button mainMenuButton = FindDeepChild(pauseMenuUI.transform, "Button_MainMenu")?.GetComponent<Button>();
        Button saveButton = FindDeepChild(pauseMenuUI.transform, "Button_Save")?.GetComponent<Button>();
        Button loadButton = FindDeepChild(pauseMenuUI.transform, "Button_Load")?.GetComponent<Button>();
        Button resumeButton = FindDeepChild(pauseMenuUI.transform, "Button_Resume")?.GetComponent<Button>();

        if (saveButton != null)
        {
            saveButton.onClick.RemoveAllListeners(); // Clear old listeners
            saveButton.onClick.AddListener(SaveButtonClicked);
        }
        else Debug.LogError("Save Button not found!");

        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(LoadButtonClicked);
        }
        else Debug.LogError("Load Button not found!");

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeButtonClicked);
        }
        else Debug.LogError("Resume Button not found!");

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(mainMenuButtonClicked);
        }
        else Debug.LogError("Resume Button not found!");
    }
}
