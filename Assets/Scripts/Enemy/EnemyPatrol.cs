using UnityEngine;

namespace Enemy
{
    public class EnemyPatrol : MonoBehaviour
    {
        private static readonly int Moving = Animator.StringToHash("moving");

        [Header("Patrol Points")]
        [SerializeField] private Transform leftEdge;
        [SerializeField] private Transform rightEdge;

        [Header("Enemy")]
        [SerializeField] private Transform enemy;

        [Header("Movement Parameters")]
        [SerializeField] private float speed;
        [SerializeField] private float idleDuration;

        [Header("Enemy Animator")]
        [SerializeField] private Animator anim;
        
        public Transform LeftEdge => leftEdge;
        public Transform RightEdge => rightEdge;

        #region Private Fields
        private Vector3 initScale;
        private bool movingLeft;
        private float idleTimer;
        #endregion
        
        private void Awake()
        {
            initScale = enemy.localScale;
        }

        private void OnDisable()
        {
            anim.SetBool(Moving, false);
        }

        private void Update()
        {
            if (movingLeft)
            {
                if (enemy.position.x >= leftEdge.position.x)
                    MoveInDirection(-1);
                else
                    DirectionChange();
            }
            else
            {
                if (enemy.position.x <= rightEdge.position.x)
                    MoveInDirection(1);
                else
                    DirectionChange();
            }
        }
        
        private void DirectionChange()
        {
            anim.SetBool(Moving, false);
            idleTimer += Time.deltaTime;

            if (idleTimer > idleDuration)
            {
                movingLeft = !movingLeft;
                idleTimer = 0;
            }
        }

        private void MoveInDirection(int direction)
        {
            idleTimer = 0;
            anim.SetBool(Moving, true);

            enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
            enemy.position = new Vector3(enemy.position.x + Time.deltaTime * direction * speed, enemy.position.y, enemy.position.z);
        }
    }
}