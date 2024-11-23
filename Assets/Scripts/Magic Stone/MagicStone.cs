using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Magic_Stone
{
    public class MagicStone : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private SpriteRenderer indicatorSprite;
        [SerializeField] private GameObject interactParticleSystemPrefab;

        private bool playerInTrigger;
        private Vector3 playerPosition;
        private GameObject activeParticleSystemInstance;
        
        private void Start()
        {
            if (indicatorSprite != null)
                indicatorSprite.enabled = false;
        }

        private void Update()
        {
            if (playerInTrigger && Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartCoroutine(PlayParticlesThenLoadLevel(playerPosition));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInTrigger = true;
                playerPosition = other.gameObject.transform.position;
                if (indicatorSprite != null)
                    indicatorSprite.enabled = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInTrigger = false;
                if (indicatorSprite != null)
                    indicatorSprite.enabled = false;
            }
        }

        #region Particle System
        private void PlayInteractParticleSystem(Vector3 position)
        {
            if (interactParticleSystemPrefab != null)
            {
                if (activeParticleSystemInstance == null || !activeParticleSystemInstance.activeInHierarchy)
                {
                    if (activeParticleSystemInstance != null)
                        Destroy(activeParticleSystemInstance);

                    activeParticleSystemInstance = Instantiate(interactParticleSystemPrefab, position + new Vector3(0, 0, -1), Quaternion.identity);
                }
            }
            else
            {
                Debug.LogError("Interact Particle System Prefab is not assigned.");
            }
        }
        #endregion
        
        private IEnumerator PlayParticlesThenLoadLevel(Vector3 position)
        {
            PlayInteractParticleSystem(position);
            yield return new WaitForSeconds(interactParticleSystemPrefab.GetComponent<ParticleSystem>().main.duration);

            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                yield return StartCoroutine(LoadSceneAndWait(SceneManager.GetActiveScene().buildIndex));
                LoadingManager.LoadSpecificLevel(0);
            }
            else
            {
                SaveGame();
                LoadingManager.LoadNextLevel();
            }
        }

        private IEnumerator LoadSceneAndWait(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
            yield return new WaitForSeconds(10);
        }
        
        private void SaveGame()
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.SaveGame();
            }
        }
    }
}