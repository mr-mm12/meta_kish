using UnityEngine;
using System.Collections;

public class AutoDoor : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform[] objectsToMove;
    public Transform[] moveTargets;

    [Header("Extra Objects")]
    public GameObject[] objectsWhenOpen;
    public GameObject[] objectsWhenClosed;

    [Header("Settings")]
    public float triggerDistance = 20f;
    public float moveDuration = 1f;

    [Header("Audio")]
    public AudioSource audioSource; // کامپوننت برای پخش صدا
    public AudioClip openSound;     // صدای باز شدن
    public AudioClip closeSound;    // صدای بسته شدن

    private Vector3[] startPositions;
    private bool isMoving = false;
    private bool isAtTarget = false;

    void Start()
    {
        if (objectsToMove.Length != moveTargets.Length)
        {
            Debug.LogError("AutoDoor: objectsToMove.Length must equal moveTargets.Length");
            enabled = false;
            return;
        }

        startPositions = new Vector3[objectsToMove.Length];
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            startPositions[i] = objectsToMove[i].position;
        }

        SetActiveObjects(false); // از اول در بسته باشه
    }

    void Update()
    {
        if (player == null || isMoving) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= triggerDistance && !isAtTarget)
        {
            StartCoroutine(MoveAll(toTarget: true));
            isAtTarget = true;
            SetActiveObjects(true);
        }
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

        // پخش صدای مناسب در شروع حرکت
        if (audioSource != null)
        {
            audioSource.clip = toTarget ? openSound : closeSound;
            audioSource.Play();
        }

        Vector3[] fromPositions = new Vector3[objectsToMove.Length];
        Vector3[] toPositions = new Vector3[objectsToMove.Length];

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            fromPositions[i] = objectsToMove[i].position;
            toPositions[i] = toTarget ? moveTargets[i].position : startPositions[i];
        }

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

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            objectsToMove[i].position = toPositions[i];
        }

        isMoving = false;
    }

    void SetActiveObjects(bool isOpen)
    {
        if (objectsWhenOpen != null)
        {
            foreach (var obj in objectsWhenOpen)
            {
                if (obj != null) obj.SetActive(isOpen);
            }
        }

        if (objectsWhenClosed != null)
        {
            foreach (var obj in objectsWhenClosed)
            {
                if (obj != null) obj.SetActive(!isOpen);
            }
        }
    }
}
