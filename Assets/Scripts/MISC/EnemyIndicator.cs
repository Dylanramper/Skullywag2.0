using UnityEngine;

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

        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        bool isOffScreen = screenPos.x <= 0 || screenPos.x >= Screen.width || screenPos.y <= 0 || screenPos.y >= Screen.height;

        arrowUI.gameObject.SetActive(isOffScreen);

        if (!isOffScreen) return;

        //Clamp Arrow to the edge of the screen
        screenPos.x = Mathf.Clamp(screenPos.x, screenEdgeOffset, Screen.width - screenEdgeOffset);
        screenPos.y = Mathf.Clamp(screenPos.y, screenEdgeOffset, Screen.height - screenEdgeOffset);

        arrowUI.position = screenPos;

        //Rotate Arrow toward enemy
        Vector3 dir = (target.position - cam.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        arrowUI.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void UpdateIndicator()
    {
        
    }

    public void Initialize(Transform newTarget)
    {
        target = newTarget;
        UpdateIndicator();
    }
}
