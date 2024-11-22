using UnityEngine;

namespace Enemy
{
    public class EnemyFireballHolder : MonoBehaviour
    {
        [SerializeField] private Transform enemy;
        
        private void Update()
        {
            UpdateScale();
        }
        
        private void UpdateScale()
        {
            transform.localScale = enemy.localScale;
        }
    }
}