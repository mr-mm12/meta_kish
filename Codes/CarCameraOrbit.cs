using UnityEngine;

public class CarCameraOrbit : MonoBehaviour
{
    [Header("References")]
    public Transform target;             // The object to orbit around (e.g., car or pivot)
    public Transform cameraTransform;   // The camera transform (e.g., Camera.main.transform)
    public Vector3 offset = new Vector3(0, 3, -6); // Offset distance of the camera from the target

    [Header("Settings")]
    public float rotationSpeed = 5f;     // Speed of camera rotation based on mouse input
    public float minY = -30f;            // Minimum vertical angle (pitch)
    public float maxY = 60f;             // Maximum vertical angle (pitch)
    public bool enableOnlyWhenDriving = true; // Enable camera control only when player is driving

    private float currentX = 0f;         // Current horizontal rotation angle (yaw)
    private float currentY = 20f;        // Current vertical rotation angle (pitch)
    private CarEntryController entryController; // Reference to car entry controller to check driving state

    private void Start()
    {
        // If no camera assigned, default to main camera transform
        if (cameraTransform == null) 
            cameraTransform = Camera.main.transform;

        // Get CarEntryController component from target to check if player is driving
        entryController = target.GetComponent<CarEntryController>();

        // Lock and hide the cursor for camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        // If control only enabled when driving and player is not in the car, do nothing
        if (enableOnlyWhenDriving && (entryController == null || !entryController.playerEnteredCar))
            return;

        // Update rotation angles based on mouse input and rotation speed
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Clamp vertical rotation angle to stay within min and max limits
        currentY = Mathf.Clamp(currentY, minY, maxY);

        // Calculate rotation quaternion from Euler angles
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Set camera position relative to target plus offset rotated by current rotation
        cameraTransform.position = target.position + rotation * offset;

        // Make the camera look slightly above the target position (e.g., car roof)
        cameraTransform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
