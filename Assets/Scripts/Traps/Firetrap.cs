using System.Collections;
using Player;
using UnityEngine;

namespace Traps
{
    public class Firetrap : MonoBehaviour
    {
        private static readonly int Activated = Animator.StringToHash("activated");
        [SerializeField] private float damage;

        [Header("Firetrap Timers")]
        [SerializeField] private float activationDelay;
        [SerializeField] private float activeTime;

        [Header("SFX")]
        [SerializeField] private AudioClip firetrapSound;
        
        private Animator anim;
        private SpriteRenderer spriteRend;
        private bool triggered; 
        private bool active;    
        private Health.Health playerHealth; 
        
        private void Awake()
        {
            InitializeComponents();
        }

        private void Update()
        {
            ApplyDamageIfActive();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.IsVisible())
            {
                SetPlayerHealth(collision);
                ActivateTrapIfNotTriggered();
                ApplyDamageIfActive();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
                playerHealth = null;
        }
        
        private void InitializeComponents()
        {
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
        }

        private void ApplyDamageIfActive()
        {
            if (playerHealth != null && active)
                playerHealth.TakeDamage(damage);
        }

        private void SetPlayerHealth(Collider2D collision)
        {
            playerHealth = collision.GetComponent<Health.Health>();
        }

        private void ActivateTrapIfNotTriggered()
        {
            if (!triggered)
                StartCoroutine(ActivateFiretrap());
        }

        private IEnumerator ActivateFiretrap()
        {
            SetTrapTriggered();
            yield return new WaitForSeconds(activationDelay);
            ActivateTrap();
            yield return new WaitForSeconds(activeTime);
            DeactivateTrap();
        }

        private void SetTrapTriggered()
        {
            triggered = true;
            spriteRend.color = Color.red;
        }

        private void ActivateTrap()
        {
            SoundManager.instance.PlaySound(firetrapSound);
            spriteRend.color = Color.white;
            active = true;
            anim.SetBool(Activated, true);
        }

        private void DeactivateTrap()
        {
            active = false;
            triggered = false;
            anim.SetBool(Activated, false);
        }
    }
}
