using UnityEngine;

public class GhostVolleyShot : MonoBehaviour
{
    [SerializeField] float speed = 7f;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }
}
