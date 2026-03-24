using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerUpUIManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject powerUpIconPrefab;   // Drag the prefab here
    public float displayDuration = 2f;     // How long icon stays before fading
    public float fadeDuration = 1f;        // How long the fade takes

    /// <summary>
    /// Call this method from your power-up script, passing the icon sprite.
    /// </summary>
    public void ShowPowerUpIcon(Sprite iconSprite)
    {
        // Create a new icon as a child of this panel
        GameObject newIcon = Instantiate(powerUpIconPrefab, transform);

        // Set the sprite on the icon's Image component
        Image iconImage = newIcon.GetComponent<Image>();
        if (iconImage != null && iconSprite != null)
        {
            iconImage.sprite = iconSprite;
        }

        // Start the fade-out coroutine
        StartCoroutine(FadeOutAndDestroy(newIcon));
    }

    IEnumerator FadeOutAndDestroy(GameObject icon)
    {
        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Get CanvasGroup (should already exist on prefab)
        CanvasGroup cg = icon.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = icon.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        // Destroy the icon
        Destroy(icon);
    }
}