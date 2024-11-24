using System.Collections;
using Core;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        
        [Header("Screens")]
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject pauseScreen;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Image loadingBarFill;

        [Header("UI Elements")]
        [SerializeField] private Image loadingImage;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private TextMeshProUGUI coinText;

        [Header("Audio")]
        [SerializeField] private AudioClip gameOverSound;

        [Header("Player Reference")]
        [SerializeField] private PlayerMovement playerMovement;

        private bool IsPauseScreenActive => pauseScreen.activeInHierarchy;
        private bool IsGameOverScreenActive => gameOverScreen.activeInHierarchy;
        
        private void Start() => CheckSaveData();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame(!IsPauseScreenActive);
            }
        }

        private void CheckSaveData()
        {
            if (GameManager.instance == null)
            {
                Debug.LogError("GameManager instance is not initialized");
                return;
            }

            if (SceneManager.GetActiveScene().buildIndex != 0) return;

            bool saveExists = GameManager.instance.SaveDataExists();

            SetButtonVisibility(continueButton, saveExists, "Continue button");
            SetButtonVisibility(newGameButton, true, "New Game button");
        }

        private static void SetButtonVisibility(Button button, bool isVisible, string buttonName)
        {
            if (button != null)
            {
                button.gameObject.SetActive(isVisible);
            }
            else
            {
                Debug.LogWarning($"{buttonName} is not assigned in the Inspector.");
            }
        }
        
        public void NewGame()
        {
            GameManager.instance.ResetCoins();
            GameManager.instance.SaveGame(true);
            UpdateCoinDisplay(0);
            StartCoroutine(LoadNewGameByIndex());
        }

        public void ContinueGame()
        {
            int lastSavedLevelIndex = GameManager.instance.GetLastSavedLevelIndex();
            LoadingManager.LoadSpecificLevel(lastSavedLevelIndex);
        }

        public void GameOver()
        {
            SetGameOverState(true);
            SoundManager.instance.PlaySound(gameOverSound);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SetGameOverState(false);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(0);
            SetGameOverState(false);
        }

        public void Quit()
        {
            GameManager.instance?.SaveGame();
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void PauseGame(bool status)
        {
            if (IsGameOverScreenActive) return;

            pauseScreen.SetActive(status);
            Time.timeScale = status ? 0.01f : 1;
            TogglePlayerMovement(!status);
        }

        public void ShowLoadingScreen(bool show)
        {
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(show);
            }
            else
            {
                Debug.LogWarning("Loading screen GameObject is not assigned in the Inspector. Creating a temporary one.");
                CreateTemporaryLoadingScreen(show);
            }
        }

        public void UpdateLoadingImage(float progress)
        {
            if (loadingImage != null)
            {
                loadingImage.fillAmount = progress;
            }
            else
            {
                Debug.LogError("Loading Image not assigned in the Inspector");
            }
        }

        public void UpdateCoinDisplay(int coins)
        {
            if (coinText != null)
            {
                coinText.text = $": {coins}";
            }
        }

        private void SetGameOverState(bool isGameOver)
        {
            gameOverScreen.SetActive(isGameOver);
            Time.timeScale = isGameOver ? 0 : 1;
            TogglePlayerMovement(!isGameOver);
        }

        private void TogglePlayerMovement(bool enable)
        {
            if (playerMovement != null)
                playerMovement.enabled = enable;
        }
        
        private IEnumerator LoadNewGameByIndex()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            ShowLoadingScreen(true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneIndex);
            if (operation != null)
            {
                operation.allowSceneActivation = false;

                while (!operation.isDone)
                {
                    float progress = Mathf.Clamp01(operation.progress / 0.9f);
                    UpdateLoadingImage(progress);

                    if (operation.progress >= 0.9f)
                    {
                        operation.allowSceneActivation = true;
                    }

                    yield return null;
                }
            }

            ShowLoadingScreen(false);
        }
        
        private void CreateTemporaryLoadingScreen(bool show)
        {
            if (show)
            {
                GameObject tempLoadingScreen = new GameObject("Temporary Loading Screen");
                Canvas canvas = tempLoadingScreen.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999;

                Image backgroundImage = tempLoadingScreen.AddComponent<Image>();
                backgroundImage.color = new Color(0, 0, 0, 0.5f);

                GameObject loadingTextObj = new GameObject("Loading Text");
                loadingTextObj.transform.SetParent(tempLoadingScreen.transform, false);
                Text loadingText = loadingTextObj.AddComponent<Text>();
                loadingText.text = "Loading...";
                loadingText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                loadingText.fontSize = 24;
                loadingText.color = Color.white;
                loadingText.alignment = TextAnchor.MiddleCenter;

                RectTransform rectTransform = loadingText.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;

                loadingScreen = tempLoadingScreen;
            }
            else if (loadingScreen != null)
            {
                Destroy(loadingScreen);
                loadingScreen = null;
            }
        }
    }
}