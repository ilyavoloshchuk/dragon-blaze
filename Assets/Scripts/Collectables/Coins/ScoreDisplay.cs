using TMPro;
using UnityEngine;

namespace Collectables.Coins
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;

        private void OnEnable()
        {
            GameManager.OnScoreChanged += UpdateScoreDisplay;
        }

        private void OnDisable()
        {
            GameManager.OnScoreChanged -= UpdateScoreDisplay;
        }

        private void UpdateScoreDisplay(int score)
        {
            if (coinText != null)
            {
                coinText.text = $": {score}";
            }
        }
    }
}
