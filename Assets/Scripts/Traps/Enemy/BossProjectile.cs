using System.Collections;
using UnityEngine;

namespace Traps.Enemy
{
    public class BossProjectile : EnemyDamage
    {
        public float speed; // Скорость снаряда
        [SerializeField] private float resetTime; // Время жизни снаряда
        private float lifetime; // Текущее время жизни
        private bool hit; // Флаг столкновения

        private Animator anim; // Анимация взрыва
        private BoxCollider2D coll; // Коллайдер снаряда
        private Vector3 _moveDirection;

        private void Awake()
        {
            InitializeComponents();
        }

        public void ActivateProjectile(Vector3 targetPosition)
        {
            // Рассчитываем направление движения
            _moveDirection = (targetPosition - transform.position).normalized;

            // Сбрасываем состояние и активируем снаряд
            ResetProjectile();

            // Запускаем корутину для движения
            StartCoroutine(MoveInDirection());
        }
        
        private void InitializeComponents()
        {
            anim = GetComponent<Animator>();
            coll = GetComponent<BoxCollider2D>();
        }

        private IEnumerator MoveInDirection()
        {
            while (!hit)
            {
                // Двигаем снаряд в указанном направлении
                float movementSpeed = speed * Time.deltaTime;
                transform.position += _moveDirection * movementSpeed;

                yield return null; // Ждём следующий кадр
            }

            // Если снаряд столкнулся, деактивируем его
            Deactivate();
        }

        private new void OnTriggerEnter2D(Collider2D collision)
        {
            if (!ShouldProcessCollision(collision)) return;

            HandleCollision();
            base.OnTriggerEnter2D(collision);
        }

        private bool ShouldProcessCollision(Collider2D collision)
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
                anim.SetTrigger("explode");
            else
                Deactivate();
        }

        private void ResetProjectile()
        {
            hit = false;
            gameObject.SetActive(true);
            coll.enabled = true;
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}