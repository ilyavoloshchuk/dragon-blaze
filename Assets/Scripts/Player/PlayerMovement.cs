using UnityEngine;
using System.Collections;
using System;
using UI;

public class PlayerMovement : MonoBehaviour
{

    #region Component References
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private PlayerRespawn playerRespawn;
    private UIManager uiManagerInstance;
    #endregion

    #region Movement
    [Header("Movement Parameters")]
    [SerializeField] public float jumpPower;
    [SerializeField] public float speed;
    private float horizontalInput;
    #endregion

    #region Coyote Time
    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;
    #endregion

    #region Multiple Jumps
    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps = 2;
    private int jumpCounter;
    #endregion

    #region Layers
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    #endregion

    #region Sounds
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    #endregion

    #region Score/Coins
    [Header("Coins")]
    private int score = 0;
    public static event Action<int> OnScoreChanged;
    #endregion

    #region Falling Parameters
    [Header("Falling Parameters")]
    [SerializeField] private float maxFallingTime = 2f;
    private float fallingTimer = 0f;
    private bool gameOverTriggered = false;
    #endregion

    #region Interactions
    private bool isInteracting;
    #endregion

    #region Particle System
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private GameObject jumpParticlesPrefab;
    [SerializeField] private GameObject wallSlideParticlesPrefab;
    private ParticleSystem wallSlideParticles;
    #endregion

    #region Dash
    [Header("Dash Effects")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private GameObject dashParticlesPrefab;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing;
    private float dashTimer;
    #endregion

    #region Fall Death
    [Header("Fall Death")]
    [SerializeField] private float deathHeight = -10f;
    private bool isDead = false;
    #endregion

    #region PowerUps
    [Header("Invisibility PowerUp")]
    [SerializeField] public float defaultInvisibilityDuration = 5f;
    [SerializeField] public Color invisibleColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] public SpriteRenderer playerSpriteRenderer;
    private bool isInvisible = false;

    [Header("Higher Jump PowerUp")]
    [SerializeField] private float defaultJumpMultiplier = 1.5f;

    [Header("Speed Boost PowerUp")]
    [SerializeField] private float defaultSpeedBoostMultiplier = 1.5f;
    [SerializeField] private float defaultSpeedBoostDuration = 5f;
    #endregion
    
    public bool OnLadder;
    

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        wallSlideParticles = Instantiate(wallSlideParticlesPrefab, transform).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!isDead)
        {
            HandlePlayerActions();
            CheckFallDeath();
        }
    }

    private void InitializeComponents()
    {
        uiManagerInstance = FindObjectOfType<UIManager>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerRespawn = GetComponent<PlayerRespawn>();
    }

    private void HandlePlayerActions()
    {
        HandleFalling();
        HandleMovement();
        HandleJump();
        HandleDash();
    }

    private void HandleFalling()
    {
        if (!IsGrounded())
        {
            fallingTimer += Time.deltaTime;

            if (fallingTimer > maxFallingTime && !gameOverTriggered && !OnLadder)
            {
                Die();
                gameOverTriggered = true;
            }
        }
        else
        {
            fallingTimer = 0f;
            gameOverTriggered = false;
        }
    }

    private void HandleMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        UpdatePlayerScale();
        UpdateAnimationState();
        ApplyMovement();
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
            anim.SetBool("grounded", true);
        }
        else
        {
            anim.SetBool("run", Mathf.Abs(horizontalInput) > 0.01f);
        }

        anim.SetBool("grounded", IsGrounded());
    }

    private void ApplyMovement()
    {
        body.gravityScale = 7;
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
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

    public bool IsInvisible()
    {
        return isInvisible;
    }

    public bool IsVisible()
    {
        return !isInvisible;
    }

    public void SetInvisibility(bool visible)
    {
        isInvisible = !visible;
        playerSpriteRenderer.color = visible ? Color.white : invisibleColor;
    }

    public void SetInteracting(bool interacting)
    {
        isInteracting = interacting;
        anim.SetBool("grounded", isInteracting);
        anim.SetBool("run", !interacting);

        if (interacting)
        {
            body.velocity = Vector2.zero;
        }
    }

    public void Die()
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

        score = 0;
        foreach (Coin coin in FindObjectsOfType<Coin>())
        {
            coin.ResetValue();
        }

        this.enabled = false;

        // Trigger death animation
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        // Disable player's collider
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // You might want to add a delay before resetting the player's position or reloading the level
        StartCoroutine(ResetPlayerAfterDelay(2f));
    }

    private IEnumerator ResetPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset player position to the last checkpoint or starting position
        if (playerRespawn != null && playerRespawn.GetCurrentCheckpoint() != null)
        {
            transform.position = playerRespawn.GetCurrentCheckpoint().position;
        }
        else
        {
            Debug.LogWarning("No checkpoint found. Resetting to default position.");
            transform.position = Vector3.zero; // Or any other default position
        }

        // Re-enable player
        isDead = false;
        this.enabled = true;

        // Re-enable player's collider
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        // Reset animation
        if (anim != null)
        {
            anim.SetTrigger("respawn");
        }
    }

    public bool CanAttack()
    {
        return Mathf.Approximately(horizontalInput, 0) && IsGrounded();
    }
}