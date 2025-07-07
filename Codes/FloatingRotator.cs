using UnityEngine;

public class FloatingRotator : MonoBehaviour
{
    // Speed of rotation around the Y axis (degrees per second)
    public float rotationSpeed = 50f;

    // Height range for floating movement (amplitude)
    public float floatAmplitude = 10f;

    // Frequency of the floating effect (how fast it oscillates)
    public float floatFrequency = 1f;

    // Initial position of the object
    private Vector3 startPos;

    void Start()
    {
        // Store the object's initial position at the start
        startPos = transform.position;
    }

    void Update()
    {
        // Continuously rotate the object around the global Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Calculate new Y position using a sine wave for smooth up and down floating
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        // Apply the new position while keeping X and Z the same
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
