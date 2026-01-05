using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 7f;
    [Range(0f, 1f)]
    public float airControl = 0.2f; // How strongly input affects movement in air

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask groundLayer;

    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;
    private bool grounded;
    private Vector3 moveDir;

    void Start()
    {
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        // Input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            // Ground: full control
            Vector3 targetVelocity = moveDir * moveSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
        else
        {
            // Air: preserve momentum, allow slight control
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (moveDir.magnitude > 0.1f)
            {
                // Blend current horizontal velocity toward input
                Vector3 adjustedVelocity = Vector3.Lerp(
                    horizontalVelocity,       // current momentum
                    moveDir.normalized * moveSpeed, // desired direction
                    airControl                // blend factor
                );

                rb.linearVelocity = new Vector3(adjustedVelocity.x, rb.linearVelocity.y, adjustedVelocity.z);
            }
            // else: no input, keep momentum
        }
    }

    private void Jump()
    {
        // Only modify vertical velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
