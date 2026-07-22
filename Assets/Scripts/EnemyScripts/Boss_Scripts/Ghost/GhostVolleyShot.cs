using System.Collections;
using UnityEngine;

public class GhostVolleyShot : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float slowSpeed = 1f;

    [SerializeField] float turnSpeed = 180f;

    private float currentSpeed;

    private Transform target;

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 1.5f;

    private bool hasExploded;

    private enum ProjectileState
    {
        Forward, Slowing, Homing, Exploding
        
    }

    private ProjectileState state;

    private void Start()
    {
        state = ProjectileState.Forward;
        currentSpeed = moveSpeed;

        StartCoroutine(StateRoutine());
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
                case ProjectileState.Forward:
                currentSpeed = moveSpeed;
                break;

                case ProjectileState.Slowing:
                currentSpeed = Mathf.Lerp(currentSpeed, slowSpeed, 6f * Time.deltaTime);
                break;

                case ProjectileState.Homing:
                currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, 6f * Time.deltaTime);

                if(target != null)
                {
                    Vector2 toTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;

                    float angle = Vector2.SignedAngle(transform.up, toTarget);

                    float turnAmount = Mathf.Clamp(angle, -turnSpeed * Time.deltaTime, turnSpeed * Time.deltaTime);

                    transform.Rotate(0f, 0f, turnAmount);

                    float distance = Vector2.Distance(transform.position, target.position);

                    if(distance < 0.2f)
                    {
                        state = ProjectileState.Exploding;
                    }
                }
                break;

                case ProjectileState.Exploding:
                if (!hasExploded)
                {
                    hasExploded = true;

                    if(explosionPrefab != null)
                    {
                        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    }
                    Destroy(target.gameObject);
                    Destroy(gameObject);
                }
                break;
        }

        transform.position += transform.up * currentSpeed * Time.deltaTime;
    }

    IEnumerator StateRoutine()
    {
        yield return new WaitForSeconds(0.10f);

        state = ProjectileState.Slowing;

        yield return new WaitForSeconds(0.10f);

        state = ProjectileState.Homing;
    }
}
