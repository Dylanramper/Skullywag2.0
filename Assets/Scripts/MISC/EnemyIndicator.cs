using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyIndicator : MonoBehaviour
{
    public Transform target;
    public RectTransform arrowUI;

    private Camera cam;

    private float screenEdgeOffset = 60f;

    [SerializeField] private RectTransform canvasRect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);
        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        bool isOffScreen = viewportPos.z < 0 || viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;

        arrowUI.GetComponent<UnityEngine.UI.Image>().enabled = isOffScreen;

        if (!isOffScreen) return;

        // Clamp arrow to edge
        screenPos.x = Mathf.Clamp(screenPos.x, screenEdgeOffset, Screen.width - screenEdgeOffset);
        screenPos.y = Mathf.Clamp(screenPos.y, screenEdgeOffset, Screen.height - screenEdgeOffset);

        arrowUI.position = screenPos;

        // Rotate arrow
        Vector3 dir = (target.position - cam.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        arrowUI.localRotation = Quaternion.Euler(0, 0, angle);

        arrowUI.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 4f) * 0.1f);
    }

    public void Initialize(Transform newTarget)
    {
        target = newTarget;
    }
}
