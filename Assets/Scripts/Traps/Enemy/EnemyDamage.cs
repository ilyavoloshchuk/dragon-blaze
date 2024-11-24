using Player;
using UnityEngine;

namespace Traps.Enemy
{
    public class EnemyDamage : MonoBehaviour
    {
        [SerializeField] protected float damage;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement == null || !playerMovement.IsVisible()) return;

            Health.Health playerHealth = collision.GetComponent<Health.Health>();
            if (playerHealth == null) return;

            playerHealth.TakeDamage(damage);
        }
    }
}