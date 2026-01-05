using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsEnemyFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float moveForce = 25f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float turnSpeed = 8f;

    [Header("Grounding")]
    [SerializeField] private bool lockYPosition = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        // Direction to target (ignore vertical)
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        direction.Normalize();

        // Apply force toward player
        rb.AddForce(direction * moveForce, ForceMode.Acceleration);

        // Clamp horizontal speed
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }

        // Smooth rotation toward movement direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.fixedDeltaTime
        );

        // Optional Y-lock to prevent drifting
        if (lockYPosition)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }
}
