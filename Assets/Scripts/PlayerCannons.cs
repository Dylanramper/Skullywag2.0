using UnityEngine;

public class PlayerCannons : MonoBehaviour
{
    public Transform leftCannonPoint;
    public Transform rightCannonPoint;
    public GameObject CannonBall;

    public float force = 8f;
    public float coolDown = 0.4f;

    private float lastFireTime;
    public void FireLeft()
    {
        if (Time.time > lastFireTime + coolDown)
            Fire(leftCannonPoint, -transform.right);
        return;
    }
    
    public void FireRight()
    {
        if (Time.time > lastFireTime + coolDown)
            Fire(rightCannonPoint, transform.right);
        return;
    }

    void Fire(Transform firePoint, Vector2 direction)
    {
        lastFireTime = Time.time;

        GameObject ball = Instantiate(CannonBall, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void testClick()
    {
        Debug.Log("Clicking");
    }
}
