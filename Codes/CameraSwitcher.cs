using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera firstPersonCamera;         // Camera for first-person view
    public Camera thirdPersonBackCamera;     // Camera behind the player
    public Camera thirdPersonFrontCamera;    // Camera in front of the player

    private int cameraState = 0;             // Current camera mode: 0 = FirstPerson, 1 = ThirdPerson Front, 2 = ThirdPerson Back

    void Start()
    {
        // Initialize camera states at the start
        SetCameraState(cameraState);
    }

    void Update()
    {
        // When the player presses the "4" key, switch to the next camera state
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            cameraState = (cameraState + 1) % 3;  // Cycle through 0 -> 1 -> 2 -> 0
            SetCameraState(cameraState);
        }
    }

    void SetCameraState(int state)
    {
        // Enable the correct camera based on the state, disable the others
        firstPersonCamera.gameObject.SetActive(state == 0);
        thirdPersonBackCamera.gameObject.SetActive(state == 1);
        thirdPersonFrontCamera.gameObject.SetActive(state == 2);
    }
}
