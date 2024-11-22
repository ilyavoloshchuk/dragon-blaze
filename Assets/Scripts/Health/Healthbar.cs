using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        [SerializeField] private Image totalhealthBar;
        [SerializeField] private Image currenthealthBar;
        
        private void Start()
        {
            InitializeTotalHealthBar();
        }

        private void Update()
        {
            UpdateCurrentHealthBar();
        }
        
        public void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            currenthealthBar.fillAmount = currentHealth / maxHealth;
        }
        
        private void InitializeTotalHealthBar()
        {
            totalhealthBar.fillAmount = playerHealth.currentHealth / 10;
        }

        private void UpdateCurrentHealthBar()
        {
            currenthealthBar.fillAmount = playerHealth.currentHealth / 10;
        }
    }
}