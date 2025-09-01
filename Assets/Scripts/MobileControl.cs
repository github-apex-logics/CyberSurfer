
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class MobileControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 20f;
    public float turnSpeed = 5f;
    public float jumpForce = 8f;
    public float gravityMultiplier = 2.5f;
    public float rampBoostMultiplier = 1.3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundCheckRadius = 0.3f;

    [Header("Touch Control")]
    public bool moveLeft = false;
    public bool moveRight = false;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 inputDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

#if UNITY_EDITOR
        // Keyboard support for testing
        inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 1).normalized;
#else
        // Touch-based input
        float xInput = 0f;
        if (moveLeft) xInput = -1f;
        if (moveRight) xInput = 1f;
        inputDir = new Vector3(xInput, 0, 1).normalized;
#endif
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplyGravity();

        if (isGrounded)
        {
            DoAutoBhop();
        }
    }

    void ApplyMovement()
    {
        // Forward movement
        Vector3 forwardMove = transform.forward * moveSpeed * Time.fixedDeltaTime;

        // Turning
        if (inputDir.x != 0)
        {
            transform.Rotate(Vector3.up * inputDir.x * turnSpeed);
        }

        // Apply velocity on ground and air
        Vector3 velocity = rb.linearVelocity;
        velocity.x = forwardMove.x;
        velocity.z = forwardMove.z;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    void DoAutoBhop()
    {
        // Raycast for detecting ramps or flat ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, groundMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            float force = angle > 5f ? jumpForce * rampBoostMultiplier : jumpForce;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reset Y
            rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    // These methods are called from UI buttons (OnPointerDown/Up)
    public void MoveLeft(bool value)
    {
        moveLeft = value;
    }

    public void MoveRight(bool value)
    {
        moveRight = value;
    }
}
