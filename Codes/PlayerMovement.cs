using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Movement speeds
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    // Acceleration times for smooth transitions
    public float walkAccelerationTime = 0.05f;
    public float runAccelerationTime = 0.4f;

    // Reference to the main camera (used for movement direction)
    public Camera cameraRef;

    private float currentSpeed = 0f;         // Current movement speed
    private float targetSpeed = 0f;          // Target speed to move towards
    private float accelerationTime = 0.1f;   // Current acceleration time

    private Vector3 moveDirection = Vector3.zero; // Final movement direction

    private Rigidbody rb;                   // Rigidbody component

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Improve physics behavior
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;    // Prevent passing through objects
        rb.interpolation = RigidbodyInterpolation.Interpolate;            // Smooth movement
        rb.constraints = RigidbodyConstraints.FreezeRotation;             // Prevent unwanted rotation
    }

    void Update()
    {
        // Read raw input from keyboard
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Get camera's forward direction and flatten it (ignore Y)
        Vector3 camForward = cameraRef.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        // Get camera's right direction and flatten it
        Vector3 camRight = cameraRef.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Combine input direction with camera orientation
        Vector3 inputDirection = camForward * moveZ + camRight * moveX;

        // Normalize if the input is significant
        if (inputDirection.magnitude > 0.01f)
            moveDirection = inputDirection.normalized;
        else
            moveDirection = Vector3.zero;

        // Check if player is walking or running
        bool isWalking = moveDirection.magnitude > 0 && !Input.GetKey(KeyCode.X);
        bool isRunning = moveDirection.magnitude > 0 && Input.GetKey(KeyCode.X);

        // Set speed and acceleration based on movement state
        if (isRunning)
        {
            targetSpeed = runSpeed;
            accelerationTime = Mathf.Max(runAccelerationTime, 0.01f);
        }
        else if (isWalking)
        {
            targetSpeed = walkSpeed;
            accelerationTime = Mathf.Max(walkAccelerationTime, 0.01f);
        }
        else
        {
            targetSpeed = 0f;
            accelerationTime = 0.1f;
        }
    }

    void FixedUpdate()
    {
        // Smoothly move current speed towards the target speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, (1f / accelerationTime) * Time.fixedDeltaTime * targetSpeed);
        Vector3 movement = moveDirection * currentSpeed;

        // Before moving, check if there's an obstacle in the way
        if (movement.magnitude > 0.01f)
        {
            RaycastHit hit;
            if (!Physics.Raycast(rb.position, movement.normalized, out hit, currentSpeed * Time.fixedDeltaTime + 0.1f))
            {
                // Move player if no obstacle is in the way
                rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
            }
            else
            {
                // Stop movement if hitting an obstacle
                currentSpeed = 0f;
            }
        }
    }
}
