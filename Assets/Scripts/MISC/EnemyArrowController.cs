using UnityEngine;
using UnityEngine.UI;

public class EnemyArrowController : MonoBehaviour
{
    [Header("Settings")]
    public Transform targetEnemy;           // The enemy this arrow follows
    public float edgeMargin = 50f;           // Distance from screen edge
    public bool hideWhenOnScreen = true;     // Hide arrow when enemy is visible

    [Header("References")]
    private RectTransform arrowRect;         // The arrow's UI rect
    private Image arrowImage;                // The arrow image (for color changes)
    private Camera mainCamera;                // Main camera reference

    void Start()
    {
        // Get components
        arrowRect = GetComponent<RectTransform>();
        arrowImage = GetComponent<Image>();
        mainCamera = Camera.main;

        if (targetEnemy == null)
        {
            Debug.LogWarning("No target enemy assigned to arrow!");
            return;
        }
    }

    void Update()
    {
        if (targetEnemy == null)
        {
            // Enemy destroyed, destroy this arrow too
            Destroy(gameObject);
            return;
        }

        UpdateArrowPosition();
    }

    void UpdateArrowPosition()
    {
        // Convert enemy world position to screen point
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetEnemy.position);

        // Check if enemy is behind the camera (z < 0)
        bool isBehind = screenPos.z < 0;
        if (isBehind)
        {
            // If behind, flip the screen position to opposite edge
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        // Check if enemy is on screen (within viewport)
        bool isOnScreen = screenPos.x > 0 && screenPos.x < Screen.width &&
                         screenPos.y > 0 && screenPos.y < Screen.height && !isBehind;

        // Hide arrow if enemy is on screen and we want to hide it
        if (hideWhenOnScreen && isOnScreen)
        {
            arrowImage.enabled = false;
            return;
        }
        else
        {
            arrowImage.enabled = true;
        }

        // Clamp position to screen edges with margin
        Vector3 clampedPos = screenPos;
        clampedPos.x = Mathf.Clamp(clampedPos.x, edgeMargin, Screen.width - edgeMargin);
        clampedPos.y = Mathf.Clamp(clampedPos.y, edgeMargin, Screen.height - edgeMargin);
        clampedPos.z = 0;

        // Convert screen position to canvas position
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponentInParent<Canvas>().GetComponent<RectTransform>(),
            clampedPos,
            null,
            out canvasPos
        );

        // Apply position
        arrowRect.anchoredPosition = canvasPos;

        // Calculate rotation to point toward enemy
        Vector2 direction = screenPos - clampedPos;
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRect.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Optional: Change color if enemy is behind
        if (isBehind)
        {
            arrowImage.color = Color.gray; // Enemy behind you
        }
        else
        {
            arrowImage.color = Color.white; // Enemy in front
        }
    }

    // Public method to set the target enemy
    public void SetTarget(Transform enemy)
    {
        targetEnemy = enemy;
    }
}