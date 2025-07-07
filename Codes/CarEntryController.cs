using UnityEngine;
using TMPro;
using System.Collections;

public class CarEntryController : MonoBehaviour
{
    [Header("References")]
    public Transform player;              // Reference to the player transform
    public Camera playerCamera;           // Player's camera
    public Camera carCamera;              // Car's camera
    public Transform carCameraTarget;    // Target position and rotation for the car camera

    [Header("Settings")]
    public float interactDistance = 60f; // Maximum distance to interact with the car

    private bool isDriving = false;       // Tracks whether the player is currently driving
    private CarAutoController carController;  // Reference to the car's controller script
    private MonoBehaviour playerController;   // Reference to the player's controller script

    public Transform interactText;        // UI element to show interaction prompt

    [Header("Audio")]
    public AudioClip enterCarSound;       // Sound played when entering the car
    private AudioSource audioSource;      // AudioSource component for playing sounds

    [Header("Status")]
    public bool playerEnteredCar = false; // Public flag indicating if player is in the car

    [Header("Objects To Toggle When Driving")]
    public GameObject[] objectsToToggle;  // Objects to activate/deactivate when driving

    private void Start()
    {
        // Get reference to CarAutoController and disable it initially
        carController = GetComponent<CarAutoController>();
        carController.enabled = false;
        carCamera.enabled = false;

        // Get player's controller script (assumed MonoBehaviour)
        playerController = player.GetComponent<MonoBehaviour>();

        // Add AudioSource component for playing sounds
        audioSource = gameObject.AddComponent<AudioSource>();

        // Initially deactivate all toggle objects
        foreach (var obj in objectsToToggle)
            if (obj != null) obj.SetActive(false);
    }

    private void Update()
    {
        // Check for interaction key press (E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            float dist = Vector3.Distance(player.position, transform.position);
            if (!isDriving && dist <= interactDistance) EnterCar();
            else if (isDriving) ExitCar();
        }

        // Show or hide interaction prompt based on player proximity and driving state
        bool showText = !isDriving && Vector3.Distance(player.position, transform.position) <= interactDistance;
        interactText.gameObject.SetActive(showText);

        // Sync car camera position and rotation with target while driving
        if (isDriving)
        {
            carCamera.transform.position = carCameraTarget.position;
            carCamera.transform.rotation = carCameraTarget.rotation;
        }
    }

    private void EnterCar()
    {
        isDriving = true;
        playerEnteredCar = true;

        // Disable player control and hide player model
        playerController.enabled = false;
        playerCamera.enabled = false;
        player.gameObject.SetActive(false);

        // Enable car control and car camera
        carController.enabled = true;
        carCamera.enabled = true;

        // Set car camera to target transform
        carCamera.transform.position = carCameraTarget.position;
        carCamera.transform.rotation = carCameraTarget.rotation;

        // Activate relevant objects (e.g., UI, effects)
        foreach (var obj in objectsToToggle)
            if (obj != null) obj.SetActive(true);

        // Play enter car sound after a short delay
        StartCoroutine(PlayEnterCarSoundDelayed(1f));
    }

    private IEnumerator PlayEnterCarSoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (enterCarSound != null)
            audioSource.PlayOneShot(enterCarSound);
    }

    // Public method to allow external calls to exit the car
    public void ExitCar()
    {
        isDriving = false;
        playerEnteredCar = false;

        // Place player beside the car when exiting
        player.position = transform.position + transform.right * 2f;
        player.gameObject.SetActive(true);

        // Re-enable player control and camera
        playerController.enabled = true;
        playerCamera.enabled = true;

        // Disable car control and camera
        carController.enabled = false;
        carCamera.enabled = false;

        // Deactivate toggle objects
        foreach (var obj in objectsToToggle)
            if (obj != null) obj.SetActive(false);
    }
}
