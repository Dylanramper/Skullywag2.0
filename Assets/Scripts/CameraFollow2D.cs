using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        //Get PlayerPos 
        //Lock movement of camera to movement of player without rotation
        if (!target) return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -10f);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
