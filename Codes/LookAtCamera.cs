using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        // Check if the main camera exists before trying to look at it
        if (Camera.main != null)
        {
            // Rotate this object to face the camera by setting its forward direction
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
