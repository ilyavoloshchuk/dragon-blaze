using System.Collections;
using Player;
using UnityEngine;

namespace Traps.Enemy
{
    public class BossProjectile : EnemyDamage
    {
        private static readonly int Explode = Animator.StringToHash("explode");
        public float speed; 
        [SerializeField] private float resetTime;
        private float lifetime; 
        private bool hit; 

        private Animator anim;
        private BoxCollider2D coll; 
        private Vector3 moveDirection;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            coll = GetComponent<BoxCollider2D>();
        }

        public void ActivateProjectile(Vector3 targetPosition)
        {
            moveDirection = (targetPosition - transform.position).normalized;
            ResetProjectile();
            StartCoroutine(MoveInDirection());
        }

        private IEnumerator MoveInDirection()
        {
            while (!hit)
            {
                var movementSpeed = speed * Time.deltaTime;
                transform.position += moveDirection * movementSpeed;

                yield return null; 
            }
            
            Deactivate();
        }

        private new void OnTriggerEnter2D(Collider2D collision)
        {
            if (!ShouldProcessCollision(collision)) return;

            HandleCollision();
            base.OnTriggerEnter2D(collision);
        }

        private static bool ShouldProcessCollision(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return true;

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            return playerMovement == null || playerMovement.IsVisible();
        }

        private void HandleCollision()
        {
            hit = true;
            coll.enabled = false;

            if (anim != null)
                anim.SetTrigger(Explode);
            else
                Deactivate();
        }

        private void ResetProjectile()
        {
            hit = false;
            gameObject.SetActive(true);
            coll.enabled = true;
        }

        private void Deactivate() =>
            gameObject.SetActive(false);
    }
}