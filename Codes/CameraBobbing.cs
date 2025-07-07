using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    public float walkSpeed = 150f;             // Not used here, but could represent character walk speed
    public float runSpeed = 400f;              // Not used here, but could represent character run speed
    public float bobAmount = 0.05f;            // Amount of vertical camera movement (bobbing)
    public float bobSpeed = 10f;               // Speed of the bobbing effect
    public float bobAmountHorizontal = 0.01f;  // Amount of horizontal sway during movement

    private float timer = 0f;                  // Timer used to drive sine/cosine calculations
    private Vector3 originalPosition;          // Original local position of the camera
    private bool isWalking = false;            // Whether the player is currently walking
    private bool isRunning = false;            // Whether the player is currently running

    void Start()
    {
        // Store the initial position of the camera
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Check if movement keys are pressed
        isWalking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
                    Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        // Check if player is running (holding movement + 'X')
        isRunning = isWalking && Input.GetKey(KeyCode.X);

        if (isWalking)
        {
            // Adjust bobbing speed and amount based on whether running or walking
            float currentBobSpeed = isRunning ? bobSpeed * 1.5f : bobSpeed;
            float currentBobAmount = isRunning ? bobAmount * 1.25f : bobAmount;

            // Advance the timer based on bobbing speed
            timer += Time.deltaTime * currentBobSpeed;

            // Calculate vertical and horizontal offsets using sine and cosine
            float bob = Mathf.Sin(timer) * currentBobAmount;
            float horizontalBob = Mathf.Cos(timer) * bobAmountHorizontal;

            // Apply bobbing effect to the camera's local position
            transform.localPosition = new Vector3(
                originalPosition.x + horizontalBob,
                originalPosition.y + bob,
                originalPosition.z
            );
        }
        else
        {
            // Reset camera position when not moving
            transform.localPosition = originalPosition;
        }
    }
}
