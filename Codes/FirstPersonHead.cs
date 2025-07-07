using UnityEngine;

public class FirstPersonHead : MonoBehaviour
{
    [Header("Settings")]
    public float sensitivity = 5f;                // Mouse sensitivity
    public Transform playerBody;                 // Reference to the player body (used for horizontal rotation)
    public string bodyLayerName = "PlayerBody";  // Layer to hide from the camera (e.g., to avoid seeing own body)

    Rigidbody bodyRb;                            // Rigidbody of the player body (optional)
    float xRotation = 0f;                         // Vertical rotation value (up/down)
    float currentMouseX = 0f;                     // Cached horizontal mouse movement
    float currentMouseY = 0f;                     // Cached vertical mouse movement

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Hide specified layer (e.g., body) from the camera's view
        int bodyLayer = LayerMask.NameToLayer(bodyLayerName);
        if (bodyLayer >= 0)
            GetComponent<Camera>().cullingMask &= ~(1 << bodyLayer);

        // Try to get the Rigidbody from the player body
        bodyRb = playerBody.GetComponent<Rigidbody>();
        if (bodyRb != null)
        {
            bodyRb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth motion
            bodyRb.freezeRotation = true;                              // Prevent physics-based rotation
        }
    }

    void Update()
    {
        // Get mouse input
        currentMouseX = Input.GetAxis("Mouse X") * sensitivity;
        currentMouseY = Input.GetAxis("Mouse Y") * sensitivity * 0.7f; // Vertical sensitivity is 30% lower

        // Update vertical camera rotation (looking up/down)
        xRotation = Mathf.Clamp(xRotation - currentMouseY, -60f, 70f); // Clamp to avoid extreme angles
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Apply vertical rotation to camera
    }

    void FixedUpdate()
    {
        if (bodyRb != null)
        {
            // Apply horizontal rotation using Rigidbody for physics-friendly control
            Quaternion deltaRot = Quaternion.Euler(0f, currentMouseX, 0f);
            bodyRb.MoveRotation(bodyRb.rotation * deltaRot);
        }
        else
        {
            // Fallback: rotate the player body manually if no Rigidbody
            playerBody.Rotate(Vector3.up * currentMouseX);
        }
    }
}
