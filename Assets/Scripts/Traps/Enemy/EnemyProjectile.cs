using UnityEngine;

namespace Traps.Enemy
{
    public class EnemyProjectile : EnemyDamage
    {
        private static readonly int Explode = Animator.StringToHash("explode");
        public float speed;
        [SerializeField] private float resetTime;
        private float lifetime;
        private bool hit;

        private Animator anim;
        private BoxCollider2D coll;
        
        private void Awake()
        {
            anim = GetComponent<Animator>();
            coll = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if (hit) return;
            MoveProjectile();
            UpdateLifetime();
        }

        private new void OnTriggerEnter2D(Collider2D collision)
        {
            if (!ShouldProcessCollision(collision)) return;

            HandleCollision();
            base.OnTriggerEnter2D(collision);
        }
        
        public void ActivateProjectile()
        {
            ResetProjectile();
        }

        private void MoveProjectile()
        {
            float movementSpeed = speed * Time.deltaTime;
            transform.Translate(movementSpeed, 0, 0);
        }

        private void UpdateLifetime()
        {
            lifetime += Time.deltaTime;
            if (lifetime > resetTime)
                Deactivate();
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
            lifetime = 0;
            gameObject.SetActive(true);
            coll.enabled = true;
        }

        private void Deactivate() =>
            gameObject.SetActive(false);
    }
}