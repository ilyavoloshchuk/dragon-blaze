using System.Collections;
using System.Collections.Generic;
using Player;
using Traps.FallingPlatform;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Health
{
    public class Health : MonoBehaviour
    {
        private static readonly int Grounded = Animator.StringToHash("grounded");
        private static readonly int Die1 = Animator.StringToHash("die");
        private static readonly int Hurt = Animator.StringToHash("hurt");

        [Header("Health")]
        [SerializeField] private float startingHealth = 100f;

        [Header("Invulnerability Frames")]
        [SerializeField] private float iFramesDuration;
        [SerializeField] private int numberOfFlashes;

        [Header("Components")]
        [SerializeField] private Behaviour[] components;

        [Header("Audio")]
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip hurtSound;

        [Header("Particle Systems")]
        [SerializeField] private GameObject hitParticleSystemPrefab;
        [SerializeField] private GameObject deathParticleSystemPrefab;

        [Header("Respawn")]
        [SerializeField] private List<FallingPlatform> fallingPlatforms;
    
        [SerializeField] private Tilemap tilemap;
    
        public float currentHealth { get; private set; }
        public Healthbar healthBar;
    
        private Animator anim;
        private SpriteRenderer spriteRend;
        private PlayerMovement playerMovement;
        private bool dead;
        private bool invulnerable;
    
        private void Awake()
        {
            currentHealth = startingHealth;
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();

            if (gameObject.CompareTag("Player"))
            {
                playerMovement = GetComponent<PlayerMovement>();
                if (playerMovement == null)
                {
                    Debug.LogError("PlayerMovement component not found on Player!");
                }
            }

            if (anim == null) Debug.LogError("Animator component not found!");
            if (spriteRend == null) Debug.LogError("SpriteRenderer component not found!");

            if (healthBar == null)
            {
                healthBar = FindObjectOfType<Healthbar>();
                if (healthBar == null)
                {
                    Debug.LogError("Healthbar component is not found in the scene.");
                }
            }
        }
    
        public void TakeDamage(float damage)
        {
            if (invulnerable || (playerMovement != null && playerMovement.IsInvisible())) return;

            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

            UpdateHealthBar();

            if (currentHealth > 0)
            {
                HandleDamage();
            }
            else
            {
                if (!dead)
                {
                    Die();
                }
            }
        }

        public void AddHealth(float value)
        {
            currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
        }

        public void Respawn()
        {
            AddHealth(startingHealth);
            ResetAnimations();
            StartCoroutine(Invulnerability());
            dead = false;

            EnableComponents();
            EnableCollider();
            ResetFallingPlatforms();
        }
    
        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.UpdateHealthUI(currentHealth, startingHealth);
            }
            else
            {
                Debug.LogError("Healthbar reference not set in Health script.");
            }
        }

        private void HandleDamage()
        {
            if (anim != null)
            {
                anim.SetTrigger(Hurt);
            }
        
            StartCoroutine(Invulnerability());
            PlaySound(hurtSound);
            SpawnParticles(hitParticleSystemPrefab);
        }

        private void Die()
        {
            DisableComponents();
            TriggerDeathAnimation();
            dead = true;
            PlaySound(deathSound);
            SpawnParticles(deathParticleSystemPrefab);

            if (gameObject.transform.parent != null)
            {
                if (gameObject.transform.parent.CompareTag("Boss"))
                {
                    RemoveTilesByPosition(19, 10, -6);
                }   
            }
        }

        private void RemoveTilesByPosition(int targetX, int startY, int endY)
        {
            for (var y = startY; y >= endY; y--) 
            {
                var position = new Vector3Int(targetX, y, 0); 
            
                if (tilemap.HasTile(position))
                    tilemap.SetTile(position, null); 
            }
        }

        private void DisableComponents()
        {
            foreach (Behaviour component in components)
            {
                if (component != null)
                {
                    component.enabled = false;
                }
                else
                {
                    Debug.LogError("Component in components array is null!");
                }
            }
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }

        private void TriggerDeathAnimation()
        {
            if (anim != null)
            {
                anim.SetBool(Grounded, true);
                anim.SetTrigger(Die1);
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySound(clip);
            }
            else
            {
                Debug.LogError("SoundManager instance not found!");
            }
        }

        private void SpawnParticles(GameObject particleSystemPrefab)
        {
            if (particleSystemPrefab != null)
            {
                Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
            }
        }

        private IEnumerator Invulnerability()
        {
            invulnerable = true;
            Physics2D.IgnoreLayerCollision(10, 11, true);

            for (int i = 0; i < numberOfFlashes; i++)
            {
                if (spriteRend != null)
                {
                    spriteRend.color = new Color(1, 0, 0, 0.5f);
                    yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
                    spriteRend.color = Color.white;
                    yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
                }
                else
                {
                    Debug.LogError("SpriteRenderer component not found!");
                }
            }

            Physics2D.IgnoreLayerCollision(10, 11, false);
            invulnerable = false;
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void ResetAnimations()
        {
            if (anim != null)
            {
                anim.ResetTrigger("die");
                anim.Play("Idle");
            }
        }

        private void EnableComponents()
        {
            foreach (Behaviour component in components)
            {
                if (component != null)
                {
                    component.enabled = true;
                }
                else
                {
                    Debug.LogError("Component in components array is null!");
                }
            }
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
        }

        private void EnableCollider()
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.enabled = true;
            }
            else
            {
                Debug.LogError("BoxCollider2D component not found!");
            }
        }

        private void ResetFallingPlatforms()
        {
            foreach (var platform in fallingPlatforms)
            {
                if (platform != null)
                {
                    platform.ResetPlatform();
                }
                else
                {
                    Debug.LogError("FallingPlatform in fallingPlatforms list is null!");
                }
            }
        }
    }
}
