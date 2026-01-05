using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 7f;
    public float airMultiplier = 0.4f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask groundLayer;

    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;

    float horizontalInput;
    float verticalInput;
    bool grounded;

    Vector3 moveDir;

    private void Start()
    {
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.2f,
            groundLayer
        );

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

    private void FixedUpdate()
    {
        if (grounded)
        {
            rb.linearVelocity = new Vector3(
                moveDir.x * moveSpeed,
                rb.linearVelocity.y,
                moveDir.z * moveSpeed
            );
        }
        else
        {
            rb.linearVelocity = new Vector3(
                moveDir.x * moveSpeed * airMultiplier,
                rb.linearVelocity.y,
                moveDir.z * moveSpeed * airMultiplier
            );
        }
    }

    private void Jump()
    {
        // reset Y velocity so jumps are consistent
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
