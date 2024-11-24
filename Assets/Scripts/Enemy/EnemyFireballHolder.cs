using UnityEngine;

namespace Enemy
{
    public class EnemyFireballHolder : MonoBehaviour
    {
        [SerializeField] private Transform enemy;
        
        private void Update()
        {
            transform.localScale = enemy.localScale;
        }
    }
}