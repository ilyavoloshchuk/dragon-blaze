using System.Collections;
using Player;
using Traps.Enemy;
using UnityEngine;

namespace Enemy
{
    public class Boss : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("rangedAttack");

        [Header("Attack Parameters")]
        [SerializeField] private float attackCooldown;
        [SerializeField] private float range;
        [SerializeField] private int damage;

        [Header("Ranged Attack")]
        [SerializeField] private Transform firepoint;
        [SerializeField] private GameObject[] fireballs;

        [Header("Collider Parameters")]
        [SerializeField] private float colliderDistance;
        [SerializeField] private BoxCollider2D boxCollider;

        [Header("Player Layer")]
        [SerializeField] private LayerMask playerLayer;

        [Header("Audio")]
        [SerializeField] private AudioClip fireballSound;
    
        private float cooldownTimer = Mathf.Infinity;
        private Animator anim;
        private EnemyPatrol enemyPatrol;
        private PlayerMovement playerMovement;
    
        private void Awake()
        {
            anim = GetComponent<Animator>();
            enemyPatrol = GetComponentInParent<EnemyPatrol>();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerMovement = player.GetComponent<PlayerMovement>();
            }
            else
            {
                Debug.LogError("Player object not found with tag 'Player'!");
            }
        }

        private void Update()
        {
            cooldownTimer += Time.deltaTime;
            
            HandleAttack();
            UpdateEnemyPatrol();
        }

        private void HandleAttack()
        {
            if (PlayerInSight() && cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger(Attack);
            }
        }

        private void UpdateEnemyPatrol()
        {
            if (enemyPatrol != null)
                enemyPatrol.enabled = !PlayerInSight();
        }
    
        private void RangedAttack()
        {
            SoundManager.instance.PlaySound(fireballSound);
            cooldownTimer = 0;
            
            Vector3 directionToPlayer = playerMovement.transform.position - transform.position;
            
            if (directionToPlayer.x < 0 && transform.localScale.x > 0 || directionToPlayer.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            
            StartCoroutine(DelayedFire());
        }
        
        private IEnumerator DelayedFire()
        {
            yield return new WaitForSeconds(0.1f);
            int fireballIndex = FindFireball();
            fireballs[fireballIndex].transform.position = firepoint.position;
            fireballs[fireballIndex].GetComponent<BossProjectile>().ActivateProjectile(playerMovement.transform.position);
        }
        
        private int FindFireball()
        {
            for (int i = 0; i < fireballs.Length; i++)
            {
                if (!fireballs[i].activeInHierarchy)
                    return i;
            }
            return 0;
        }
    
        private bool PlayerInSight()
        {
            if (playerMovement.IsInvisible()) return false;

            RaycastHit2D hit = Physics2D.BoxCast(
                boxCollider.bounds.center + transform.right * (range * transform.localScale.x * colliderDistance),
                new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
                0, Vector2.left, 0, playerLayer);

            return hit.collider != null;
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
        }   
    }
}