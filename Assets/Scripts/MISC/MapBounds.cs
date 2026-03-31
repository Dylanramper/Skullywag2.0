using UnityEngine;

public class MapBounds : MonoBehaviour
{
    public static MapBounds Instance;

    public Vector2 center = Vector2.zero; // center of your map/donut
    public float innerRadius = 50f;       // inner radius of your donut

    private void Awake()
    {
        Instance = this;
    }

    // Returns true if object is too close to the storm
    public bool IsNearEdge(Vector2 pos, float buffer)
    {
        float distance = Vector2.Distance(pos, center);
        return distance >= (innerRadius - buffer);
    }

    // Returns the direction to push the object back toward the center
    public Vector2 GetDirectionToCenter(Vector2 pos)
    {
        return (center - pos).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, innerRadius);
    }
}