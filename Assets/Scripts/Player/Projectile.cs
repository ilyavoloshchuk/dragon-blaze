using UnityEngine;

namespace Player
{
    public class Projectile : MonoBehaviour
    {
        private static readonly int Explode = Animator.StringToHash("explode");
        [SerializeField] private float speed;
        
        private float _direction;
        private bool hit;
        private float lifetime;

        private Animator anim;
        private BoxCollider2D boxCollider;
        
        private void Awake()
        {
            anim = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if (hit) return;

            MoveProjectile();
            UpdateLifetime();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            HandleCollision(collision);
        }
        
        public void SetDirection(float direction)
        {
            ResetProjectile(direction);
            FlipProjectile(direction);
        }

        private void MoveProjectile()
        {
            float movementSpeed = speed * Time.deltaTime * _direction;
            transform.Translate(movementSpeed, 0, 0);
        }

        private void UpdateLifetime()
        {
            lifetime += Time.deltaTime;
            if (lifetime > 5) Deactivate();
        }

        private void HandleCollision(Collider2D collision)
        {
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger(Explode);

            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Health>()?.TakeDamage(1);
            }
        }

        private void ResetProjectile(float direction)
        {
            lifetime = 0;
            _direction = direction;
            gameObject.SetActive(true);
            hit = false;
            boxCollider.enabled = true;
        }

        private void FlipProjectile(float direction)
        {
            float localScaleX = transform.localScale.x;
            if (!Mathf.Approximately(Mathf.Sign(localScaleX), direction))
            {
                localScaleX = -localScaleX;
            }
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
