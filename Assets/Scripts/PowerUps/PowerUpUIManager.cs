using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PowerUpUIManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject powerUpIconPrefab;   // Drag the prefab here
    public float displayDuration = 8f;     // How long icon stays before fading
    public float fadeDuration = 2f;        // How long the fade takes

    public static PowerUpUIManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
        // Wait before fading
        yield return new WaitForSeconds(displayDuration);

        //Check if icon still exists
        if (icon == null)
            yield break;

        CanvasGroup cg = icon.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = icon.AddComponent<CanvasGroup>();

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            //Check every frame
            if (icon == null)
                yield break;

            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
    }
}