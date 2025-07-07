using UnityEngine;
using System.Collections;

public class AutoDoor : MonoBehaviour
{
    [Header("References")]
    public Transform player;                    // Reference to the player
    public Transform[] objectsToMove;           // Door parts or objects that will move
    public Transform[] moveTargets;             // Target positions to move objects to

    [Header("Extra Objects")]
    public GameObject[] objectsWhenOpen;        // Objects to activate when the door is open
    public GameObject[] objectsWhenClosed;      // Objects to activate when the door is closed

    [Header("Settings")]
    public float triggerDistance = 20f;         // Distance at which the door is triggered
    public float moveDuration = 1f;             // Time it takes to move the door

    [Header("Audio")]
    public AudioSource audioSource;             // Audio source for playing sounds
    public AudioClip openSound;                 // Sound when opening
    public AudioClip closeSound;                // Sound when closing

    private Vector3[] startPositions;           // Original positions of door objects
    private bool isMoving = false;              // Whether the door is currently moving
    private bool isAtTarget = false;            // Whether the door is currently in open position

    void Start()
    {
        // Ensure the number of objects matches the number of target positions
        if (objectsToMove.Length != moveTargets.Length)
        {
            Debug.LogError("AutoDoor: objectsToMove.Length must equal moveTargets.Length");
            enabled = false;
            return;
        }

        // Store initial positions of the objects
        startPositions = new Vector3[objectsToMove.Length];
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            startPositions[i] = objectsToMove[i].position;
        }

        // Initially set door to closed state
        SetActiveObjects(false);
    }

    void Update()
    {
        // Do nothing if player is not assigned or door is already moving
        if (player == null || isMoving) return;

        // Calculate distance between player and this object
        float dist = Vector3.Distance(player.position, transform.position);

        // If player is close and door is not yet open, open the door
        if (dist <= triggerDistance && !isAtTarget)
        {
            StartCoroutine(MoveAll(toTarget: true));
            isAtTarget = true;
            SetActiveObjects(true);
        }
        // If player moves away and door is open, close the door
        else if (dist > triggerDistance && isAtTarget)
        {
            StartCoroutine(MoveAll(toTarget: false));
            isAtTarget = false;
            SetActiveObjects(false);
        }
    }

    IEnumerator MoveAll(bool toTarget)
    {
        isMoving = true;

        // Play appropriate sound at the start of the movement
        if (audioSource != null)
        {
            audioSource.clip = toTarget ? openSound : closeSound;
            audioSource.Play();
        }

        // Prepare from and to positions
        Vector3[] fromPositions = new Vector3[objectsToMove.Length];
        Vector3[] toPositions = new Vector3[objectsToMove.Length];

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            fromPositions[i] = objectsToMove[i].position;
            toPositions[i] = toTarget ? moveTargets[i].position : startPositions[i];
        }

        // Smoothly interpolate object positions over time
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            for (int i = 0; i < objectsToMove.Length; i++)
            {
                objectsToMove[i].position = Vector3.Lerp(fromPositions[i], toPositions[i], t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            objectsToMove[i].position = toPositions[i];
        }

        isMoving = false;
    }

    void SetActiveObjects(bool isOpen)
    {
        // Activate objects that should be shown when door is open
        if (objectsWhenOpen != null)
        {
            foreach (var obj in objectsWhenOpen)
            {
                if (obj != null) obj.SetActive(isOpen);
            }
        }

        // Activate objects that should be shown when door is closed
        if (objectsWhenClosed != null)
        {
            foreach (var obj in objectsWhenClosed)
            {
                if (obj != null) obj.SetActive(!isOpen);
            }
        }
    }
}
