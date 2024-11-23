using UnityEngine;

namespace UI
{
    public class MenuParallax : MonoBehaviour
    {
        [SerializeField] private float offsetMultiplier = 1f;
        [SerializeField] private float smoothTime = 0.3f;
        
        private Vector2 startPosition;
        private Vector3 velocity;
        
        private void Start() =>
            startPosition = transform.position;

        private void Update()
        {
            Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector2 targetPosition = startPosition + (offset * offsetMultiplier);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
