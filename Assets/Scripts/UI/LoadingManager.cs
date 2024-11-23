using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadingManager : MonoBehaviour
    {
        private static LoadingManager _instance;

        private static LoadingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LoadingManager>();
                    if (_instance == null)
                    {
                        var obj = new GameObject
                        {
                            name = nameof(LoadingManager)
                        };
                        _instance = obj.AddComponent<LoadingManager>();
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        [Header("Scene Loading Settings")]
        [SerializeField] private UIManager uiManager;
        
        public static void LoadNextLevel()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            Instance.StartCoroutine(Instance.LoadLevel(nextSceneIndex));
        }

        public static void LoadSpecificLevel(int levelIndex)
        {
            Instance.StartCoroutine(Instance.LoadLevel(levelIndex));
        }
        
        private IEnumerator LoadLevel(int levelIndex)
        {
            EnsureUIManager();

            uiManager.ShowLoadingScreen(true);

            AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
            if (operation != null)
            {
                operation.allowSceneActivation = false;

                while (!operation.isDone)
                {
                    float progress = Mathf.Clamp01(operation.progress / 0.9f);
                    uiManager.UpdateLoadingImage(progress);

                    if (operation.progress >= 0.9f)
                    {
                        yield return new WaitForSeconds(0.5f);
                        operation.allowSceneActivation = true;
                    }

                    yield return null;
                }
            }

            uiManager.ShowLoadingScreen(false);
        }

        private void EnsureUIManager()
        {
            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIManager>();
                if (uiManager == null)
                {
                    Debug.LogWarning("UIManager not found in the scene. Creating a temporary one.");
                    GameObject tempUIManager = new GameObject("Temporary UIManager");
                    uiManager = tempUIManager.AddComponent<UIManager>();
                }
            }
        }
    }
}