using UnityEngine;

public class BossIndicator : MonoBehaviour
{
    public Transform boss;
    public Camera cam;
    public RectTransform indicator;

    private bool isActive = false;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        indicator.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isActive || boss == null || cam == null) return;

        Vector3 screenPos = cam.WorldToViewportPoint(boss.position);

        bool isOffScreen = screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1;

        //  Properly hide/show
        indicator.gameObject.SetActive(isOffScreen);

        if (!isOffScreen) return;

        // Clamp to screen edge
        screenPos.x = Mathf.Clamp(screenPos.x, 0.05f, 0.95f);
        screenPos.y = Mathf.Clamp(screenPos.y, 0.05f, 0.95f);

        Vector3 worldPos = cam.ViewportToScreenPoint(screenPos);
        indicator.position = worldPos;

        // Rotate toward boss
        Vector3 dir = boss.position - cam.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        indicator.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    //  Call this when boss spawns
    public void SetBoss(Transform bossTransform)
    {
        boss = bossTransform;
        isActive = true;
    }

    //  Call this when boss dies
    public void ClearBoss()
    {
        boss = null;
        isActive = false;
        indicator.gameObject.SetActive(false);
    }
}