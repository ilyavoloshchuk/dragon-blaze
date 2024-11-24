using System;
using System.IO;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    [Serializable]
    public class SaveData
    {
        public int totalCoins;
        public int currentLevel;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        private void Awake()
        {
            InitializeSingleton();
            InitializeSaveSystem();
        }

        private void InitializeSingleton()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSaveSystem()
        {
            saveFilePath = Application.persistentDataPath + "/savefile.json";
            LoadGame();
        }
        
        private int totalCoins;

        private string saveFilePath;
        
        public static event Action<int> OnScoreChanged;

        public void AddCoins(int value)
        {
            totalCoins += value;
            OnScoreChanged?.Invoke(totalCoins);
            SaveGame();
            UpdateUICoins();
        }

        public void ResetCoins()
        {
            totalCoins = 0;
            OnScoreChanged?.Invoke(totalCoins);
            SaveGame();
            UpdateUICoins();
        }

        private void UpdateUICoins()
        {
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
                uiManager.UpdateCoinDisplay(totalCoins);
            else
                Debug.LogError("UIManager not found.");
        }
        
        public void SaveGame(bool isNewGame = false)
        {
            SaveData data = new SaveData
            {
                totalCoins = totalCoins,
                currentLevel = isNewGame ? 1 : SceneManager.GetActiveScene().buildIndex
            };
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(saveFilePath, json);
        }

        public bool SaveDataExists()
        {
            return File.Exists(saveFilePath);
        }

        private void LoadGame()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                totalCoins = data.totalCoins;
            }
        }

        public int GetLastSavedLevelIndex()
        {
            SaveData saveData = LoadSaveData();
            return saveData?.currentLevel ?? 1;
        }

        private SaveData LoadSaveData()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            return null;
        }
    }
}