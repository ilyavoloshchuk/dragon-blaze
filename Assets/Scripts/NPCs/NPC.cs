using UnityEngine;
using UnityEngine.InputSystem;

namespace NPCs
{
    public abstract class Npc : MonoBehaviour
    {
        private const float InteractDistance = 5f;
        [SerializeField] private SpriteRenderer _interactSprite;
        
        private Transform playerTransform;
        
        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    
        private void Update()
        {
            HandleInteraction();
            UpdateInteractSprite();
        }

        protected abstract void Interact();
        
        private void HandleInteraction()
        {
            if (Keyboard.current.eKey.wasPressedThisFrame && IsWithinInteractDistance())
            {
                Interact();
            }
        }

        private void UpdateInteractSprite()
        {
            bool shouldBeActive = IsWithinInteractDistance();
            if (_interactSprite.gameObject.activeSelf != shouldBeActive)
            {
                _interactSprite.gameObject.SetActive(shouldBeActive);
            }
        }

        private bool IsWithinInteractDistance()
        {
            return Vector2.Distance(playerTransform.position, transform.position) < InteractDistance;
        }
    }
}
