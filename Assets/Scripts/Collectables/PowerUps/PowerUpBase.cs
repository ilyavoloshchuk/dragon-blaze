using System.Collections;
using Player;
using UnityEngine;

namespace Collectables.PowerUps
{
    public abstract class PowerUpBase : MonoBehaviour
    {
        private const float Duration = 5f;
        private Coroutine powerUpCoroutine;
        private SpriteRenderer spriteRenderer;
        
        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                ActivatePowerUp(playerMovement);
                if (powerUpCoroutine != null)
                    StopCoroutine(powerUpCoroutine);
                powerUpCoroutine = StartCoroutine(PowerUpTimer(playerMovement));
                StartCoroutine(FadeOutAndInSprite());

                GetComponent<Collider2D>().enabled = false;
            }
        }
        
        protected abstract void ActivatePowerUp(PlayerMovement playerMovement);
        protected abstract void DeactivatePowerUp(PlayerMovement playerMovement);

        protected void ActivateIndicator(string powerUpName, Sprite powerUpImage)
        {
            PowerUpIndicatorManager indicatorManager = FindObjectOfType<PowerUpIndicatorManager>();
            if (indicatorManager != null)
            {
                indicatorManager.ActivateIndicator(powerUpName, powerUpImage, Duration);
            }
            else
            {
                Debug.LogWarning("PowerUpIndicatorManager not found in the scene.");
            }
        }

        private IEnumerator FadeOutAndInSprite()
        {
            for (float i = 1f; i >= 0; i -= Time.deltaTime)
            {
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = i;
                    spriteRenderer.color = c;
                }
                yield return null;
            }
            
            yield return new WaitForSeconds(Duration);
            
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = i;
                    spriteRenderer.color = c;
                }
                yield return null;
            }
        }

        private IEnumerator PowerUpTimer(PlayerMovement playerMovement)
        {
            yield return new WaitForSeconds(Duration);
            DeactivatePowerUp(playerMovement);
            GetComponent<Collider2D>().enabled = true;
        }
    }
}
