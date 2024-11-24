using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Collectables.PowerUps
{
    public class PowerUpIndicatorManager : MonoBehaviour
    {
        [Header("References")]
        public GameObject indicatorPrefab;
        public Transform indicatorsPanel;

        private const float InitialOpacity = 0.5f;

        private readonly List<GameObject> activeIndicators = new();

        public void ActivateIndicator(string powerUpName, Sprite powerUpImage, float duration)
        {
            if (!ValidateReferences()) return;

            GameObject existingIndicator = FindIndicatorByName(powerUpName);
            if (existingIndicator != null)
            {
                ResetExistingIndicator(existingIndicator, duration);
                return;
            }

            CreateNewIndicator(powerUpName, powerUpImage, duration);
        }

        private bool ValidateReferences()
        {
            if (indicatorPrefab == null)
            {
                Debug.LogError("indicatorPrefab is not set.");
                return false;
            }

            if (indicatorsPanel != null) return true;
            
            Debug.LogError("indicatorsPanel is not set.");
            
            return false;
        }

        private void ResetExistingIndicator(GameObject indicator, float duration)
        {
            indicator.SetActive(true);
            Image imageComponent = indicator.GetComponentInChildren<Image>();
            StopCoroutine(UpdateIndicator(indicator, duration, imageComponent));
            StartCoroutine(UpdateIndicator(indicator, duration, imageComponent));
        }

        private void CreateNewIndicator(string powerUpName, Sprite powerUpImage, float duration)
        {
            GameObject newIndicator = Instantiate(indicatorPrefab, indicatorsPanel);
            if (newIndicator == null)
            {
                Debug.LogError("Failed to instantiate newIndicator.");
                return;
            }

            SetupIndicatorComponents(newIndicator, powerUpName, powerUpImage, duration);
            activeIndicators.Add(newIndicator);
        }

        private void SetupIndicatorComponents(GameObject indicator, string powerUpName, Sprite powerUpImage, float duration)
        {
            Image imageComponent = indicator.transform.Find("Image").GetComponent<Image>();
            if (imageComponent == null)
            {
                Debug.LogError("Image component not found in indicator.");
                return;
            }

            imageComponent.sprite = powerUpImage;
            imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, InitialOpacity);

            var textComponent = indicator.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = $"<b><size=120%>{powerUpName}</size></b>";
            }
            else
            {
                Debug.LogError("Text component not found in indicator.");
            }

            StartCoroutine(UpdateIndicator(indicator, duration, imageComponent));
        }

        private GameObject FindIndicatorByName(string powerUpName)
        {
            return activeIndicators.Find(indicator => 
            {
                var textComponent = indicator.GetComponentInChildren<TMP_Text>();
                return textComponent != null && textComponent.text.Contains(powerUpName);
            });
        }

        private IEnumerator UpdateIndicator(GameObject indicator, float duration, Image imageComponent)
        {
            float remainingTime = duration;
            while (remainingTime > 0)
            {
                if (imageComponent != null)
                {
                    float alpha = remainingTime / duration;
                    imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, alpha);
                }
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            activeIndicators.Remove(indicator);
            Destroy(indicator);
        }
    }
}
