using UI;
using UnityEngine;

namespace Player
{
    public class PlayerRespawn : MonoBehaviour
    {
        private static readonly int Activate = Animator.StringToHash("activate");
        [SerializeField] private AudioClip checkpoint;
        
        private Transform currentCheckpoint;
        private Health playerHealth;
        private UIManager uiManager;
        
        private void Awake()
        {
            InitializeComponents();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            HandleCheckpointCollision(collision);
        }
        
        public void RespawnCheck()
        {
            if (currentCheckpoint == null)
            {
                uiManager.GameOver();
                return;
            }

            RespawnPlayer();
        }

        public Transform GetCurrentCheckpoint()
        {
            return currentCheckpoint;
        }
        
        private void InitializeComponents()
        {
            playerHealth = GetComponent<Health>();
            uiManager = FindObjectOfType<UIManager>();
        }

        private void HandleCheckpointCollision(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Checkpoint"))
            {
                SetCheckpoint(collision);
                ActivateCheckpoint(collision);
            }
        }

        private void SetCheckpoint(Collider2D checkpoint)
        {
            currentCheckpoint = checkpoint.transform;
            SoundManager.instance.PlaySound(this.checkpoint);
        }

        private static void ActivateCheckpoint(Collider2D checkpoint)
        {
            checkpoint.enabled = false;
            checkpoint.GetComponent<Animator>().SetTrigger(Activate);
        }

        private void RespawnPlayer()
        {
            playerHealth.Respawn();
            transform.position = currentCheckpoint.position;
        }
    }
}