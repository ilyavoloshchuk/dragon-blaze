using System.Collections;
using Collectables.Coins;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private static readonly int Grounded = Animator.StringToHash("grounded");
        private static readonly int Run = Animator.StringToHash("run");
        private static readonly int Die1 = Animator.StringToHash("die");
        private static readonly int Respawn = Animator.StringToHash("respawn");
        private Rigidbody2D body;
        private Animator anim;
        private BoxCollider2D boxCollider;
        private PlayerRespawn playerRespawn;
        private UIManager uiManagerInstance;
        
        [Header("Movement Parameters")]
        [SerializeField] public float jumpPower;
        [SerializeField] public float speed;
        private float horizontalInput;
        
        [Header("Coyote Time")]
        [SerializeField] private float coyoteTime;
        private float coyoteCounter;
        
        [Header("Multiple Jumps")]
        [SerializeField] private int extraJumps = 2;
        private int jumpCounter;
        
        [Header("Layers")]
        [SerializeField] private LayerMask groundLayer;
        
        [Header("Sounds")]
        [SerializeField] private AudioClip jumpSound;
        
        [Header("Falling Parameters")]
        [SerializeField] private float maxFallingTime = 2f;
        private float fallingTimer;
        private bool gameOverTriggered;
        
        private bool isInteracting;
        
        [SerializeField] private GameObject deathParticlesPrefab;
        [SerializeField] private GameObject jumpParticlesPrefab;
        [SerializeField] private GameObject wallSlideParticlesPrefab;
        
        [Header("Dash Effects")]
        [SerializeField] private AudioClip dashSound;
        [SerializeField] private GameObject dashParticlesPrefab;
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.2f;
        private bool isDashing;
        private float dashTimer;
        
        [Header("Fall Death")]
        [SerializeField] private float deathHeight = -10f;
        private bool isDead;
    
        [Header("Invisibility PowerUp")]
        [SerializeField] public Color invisibleColor = new(1f, 1f, 1f, 0.5f);
        [SerializeField] public SpriteRenderer playerSpriteRenderer;
        private bool isInvisible;


        private void Awake()
        {
            uiManagerInstance = FindObjectOfType<UIManager>();
            body = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
            playerRespawn = GetComponent<PlayerRespawn>();
        }

        private void Start()
        {
            Instantiate(wallSlideParticlesPrefab, transform).GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!isDead)
            {
                HandlePlayerActions();
                CheckFallDeath();
            }
        }

        private void HandlePlayerActions()
        {
            HandleMovement();
            HandleJump();
            HandleDash();
        }

        private void HandleMovement()
        {
            horizontalInput = Input.GetAxis("Horizontal");

            UpdatePlayerScale();
            UpdateAnimationState();
            
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        }

        private void UpdatePlayerScale()
        {
            if (horizontalInput > 0.01f)
            {
                transform.localScale = Vector3.one;
            }
            else if (horizontalInput < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private void UpdateAnimationState()
        {
            if (isInteracting)
            {
                anim.SetBool(Grounded, true);
            }
            else
            {
                anim.SetBool(Run, Mathf.Abs(horizontalInput) > 0.01f);
            }

            anim.SetBool(Grounded, IsGrounded());
        }
        
        private void HandleJump()
        {
            if ((IsGrounded() || coyoteCounter > 0 || jumpCounter > 0) && Input.GetKeyDown(KeyCode.Space))
                Jump();

            if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
                body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

            if (IsGrounded())
            {
                ResetJumpState();
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }
        }

        private void ResetJumpState()
        {
            if (IsGrounded())
            {
                coyoteCounter = coyoteTime;
                jumpCounter = extraJumps;
                body.gravityScale = 1;
            }
        }

        private void Jump()
        {
            if (IsGrounded() || coyoteCounter > 0)
            {
                PerformJump();
                jumpCounter = extraJumps;
            }
            else if (jumpCounter > 0)
            {
                PerformJump();
                jumpCounter--;
            }
        }

        private void PerformJump()
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            Instantiate(jumpParticlesPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        }

        private bool IsGrounded()
        {
            float extraHeight = 0.1f;
            int groundLayerMask = LayerMask.GetMask("Ground");
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, groundLayerMask);
            return raycastHit.collider != null;
        }

        private void HandleDash()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && Mathf.Abs(horizontalInput) > 0.01f)
            {
                PlayDashEffects();
                if (!isDashing)
                {
                    StartCoroutine(Dash());
                }
            }
        }

        private void PlayDashEffects()
        {
            if (dashSound != null)
            {
                AudioSource.PlayClipAtPoint(dashSound, transform.position);
            }
            else
            {
                Debug.LogWarning("Dash sound clip is not assigned.");
            }

            if (dashParticlesPrefab != null)
            {
                Instantiate(dashParticlesPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Dash Particle System Prefab is not assigned.");
            }
        }

        private IEnumerator Dash()
        {
            isDashing = true;
            float originalSpeed = speed;
            speed = dashSpeed;

            Color originalColor = playerSpriteRenderer.color;
            playerSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

            yield return new WaitForSeconds(dashDuration);

            playerSpriteRenderer.color = originalColor;
            speed = originalSpeed;
            isDashing = false;
        }

        private void CheckFallDeath()
        {
            if (transform.position.y < deathHeight)
            {
                Die();
            }
        }

        public bool IsInvisible() =>
            isInvisible;

        public bool IsVisible() =>
            !isInvisible;

        public void SetInvisibility(bool visible)
        {
            isInvisible = !visible;
            playerSpriteRenderer.color = visible ? Color.white : invisibleColor;
        }

        public void SetInteracting(bool interacting)
        {
            isInteracting = interacting;
            anim.SetBool(Grounded, isInteracting);
            anim.SetBool(Run, !interacting);

            if (interacting)
            {
                body.velocity = Vector2.zero;
            }
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;

            if (uiManagerInstance != null)
            {
                uiManagerInstance.GameOver();
            }
            else
            {
                Debug.LogWarning("UIManager instance is not found!");
            }

            if (deathParticlesPrefab != null)
            {
                Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Death Particle System Prefab is not assigned.");
            }
        
            foreach (Coin coin in FindObjectsOfType<Coin>())
            {
                coin.ResetValue();
            }

            enabled = false;
            
            if (anim != null)
            {
                anim.SetTrigger(Die1);
            }
            
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = false;
            }
            
            StartCoroutine(ResetPlayerAfterDelay(2f));
        }

        private IEnumerator ResetPlayerAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (playerRespawn != null && playerRespawn.GetCurrentCheckpoint() != null)
            {
                transform.position = playerRespawn.GetCurrentCheckpoint().position;
            }
            else
            {
                Debug.LogWarning("No checkpoint found. Resetting to default position.");
                transform.position = Vector3.zero; 
            }
            
            isDead = false;
            enabled = true;
            
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = true;
            }
            
            if (anim != null)
            {
                anim.SetTrigger(Respawn);
            }
        }

        public bool CanAttack() => 
            Mathf.Approximately(horizontalInput, 0) && IsGrounded();
    }
}