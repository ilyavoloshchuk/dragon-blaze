using UnityEngine;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("attack");
        [SerializeField] private float attackCooldown;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject[] fireballs;
        [SerializeField] private AudioClip fireballSound;

        private Animator anim;
        private PlayerMovement playerMovement;
        private float cooldownTimer = Mathf.Infinity;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            cooldownTimer += Time.deltaTime;
            
            if (CanAttack())
            {
                if (!ValidateAttackComponents())
                {
                    return;
                }

                PerformAttack();
            }
        }

        private bool CanAttack() =>
            Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.CanAttack() && Time.timeScale > 0;

        private bool ValidateAttackComponents()
        {
            if (SoundManager.instance == null)
            {
                Debug.LogError("SoundManager instance is not initialized.");
                return false;
            }
            if (fireballs == null || fireballs.Length == 0)
            {
                Debug.LogError("Fireballs array is not initialized or empty.");
                return false;
            }

            if (firePoint != null) return true;
            
            Debug.LogError("FirePoint is not assigned.");
            
            return false;
        }

        private void PerformAttack()
        {
            SoundManager.instance.PlaySound(fireballSound);
            anim.SetTrigger(Attack);
            cooldownTimer = 0;

            LaunchFireball();
        }

        private void LaunchFireball()
        {
            int fireballIndex = FindFireball();
            if (fireballIndex != -1 && fireballs[fireballIndex] != null)
            {
                GameObject fireball = fireballs[fireballIndex];
                fireball.transform.position = firePoint.position;
                fireball.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
            }
            else
            {
                Debug.LogError("Invalid fireball index or fireball is null.");
            }
        }

        private int FindFireball()
        {
            for (int i = 0; i < fireballs.Length; i++)
            {
                if (!fireballs[i].activeInHierarchy)
                    return i;
            }
            return -1;
        }
    }
}
