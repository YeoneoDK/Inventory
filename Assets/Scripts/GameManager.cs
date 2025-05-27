using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameData gameData;

    public int selectedCharacterNumber;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager instance initialized.");
            ItemDatabase.Initialize(); // Ensure items are loaded first
        }
        else
        {
            Debug.Log("GameManager instance already exists. Destroying this one.");
            Destroy(gameObject);
        }
        Debug.Log("Existing GameManager: " + instance.gameObject.name);
        Debug.Log("New GameManager: " + gameObject.name);
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    public void SetDifficulty(string difficulty)
    {
        gameData.difficulty = difficulty;
    }

    public void SetGameMode(string gameMode)
    {
        gameData.gameMode = gameMode;
    }

    public void SaveGame()
    {
        if (instance == null)
        {
            Debug.LogError("GameManager instance is null");
            return;
        }
        GameSaveManager.instance.SaveGame();
    }

    public void LoadGame(string saveFileName)
    {
        gameData = GameSaveManager.instance.LoadGame(saveFileName);
        if (gameData == null)
        {
            gameData = new GameData(); // Create new game data if no file is found
        }
    }


    public void ApplyLoadedGameData(GameData data)
    {
        if (data != null)
        {
            gameData = data;
            // Apply data to the game (e.g., characters, inventory, game state)
        }
    }


    public void AddDefaultItemsToVault()
    {
        if (gameData.equipmentVaultCodes.Count > 0) return; // Prevent duplicates

        Debug.Log("Adding default items to vault.");

        gameData.equipmentVaultCodes.Add(2);
        gameData.equipmentVaultCodes.Add(1);
        gameData.equipmentVaultCodes.Add(0);

        Debug.Log("Default vault items added: " + gameData.equipmentVaultCodes.Count);
    }
}
