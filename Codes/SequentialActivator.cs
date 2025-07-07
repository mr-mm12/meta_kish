using UnityEngine;

public class SequentialActivator : MonoBehaviour
{
    public Transform playerTransform;        // Reference to the player transform
    public GameObject[] objects;             // Array of objects to activate one-by-one
    public float triggerDistance = 30f;      // Distance required to activate the next object

    private int currentIndex = 0;            // Index of the currently active object
    private bool isWaitingForNext = false;   // Flag to prevent overlapping triggers

    void Start()
    {
        // Deactivate all objects except the first one
        for (int i = 0; i < objects.Length; i++)
            objects[i].SetActive(i == 0);
    }

    void Update()
    {
        // If all objects have been processed, stop checking
        if (currentIndex >= objects.Length) return;

        // Measure distance between player and current object
        float distance = Vector3.Distance(playerTransform.position, objects[currentIndex].transform.position);

        // If within trigger distance and not already waiting, trigger next activation
        if (distance <= triggerDistance && !isWaitingForNext)
        {
            isWaitingForNext = true;
            StartCoroutine(ActivateNextAfterDelay(0.3f)); // Small delay to keep sequence smooth
        }
    }

    System.Collections.IEnumerator ActivateNextAfterDelay(float delay)
    {
        // Wait for specified delay before activating next
        yield return new WaitForSeconds(delay);

        // Deactivate current object
        objects[currentIndex].SetActive(false);

        // Move to the next object
        currentIndex++;
        isWaitingForNext = false;

        // Activate the next object if it exists
        if (currentIndex < objects.Length)
            objects[currentIndex].SetActive(true);
    }
}
