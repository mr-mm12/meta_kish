using UnityEngine;
using System.Collections;

public class PhoneController : MonoBehaviour {
    public GameObject phoneObject;
    public Transform cameraTransform;
    public Vector3 offset = new Vector3(5.4f, -1.15f, 1.5f);
    public Vector3 rotationOffset = new Vector3(-90f, 0f, 10f);

    public float animationDuration = 0.3f;
    private bool isAnimating = false;

    
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    void Awake() {
        if (cameraTransform == null && Camera.main != null) {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P) && !isAnimating) {
            if (phoneObject != null) {
                if (phoneObject.activeSelf) {
                    StartCoroutine(AnimateHide());
                } else {
                    StartCoroutine(AnimateShow());
                }
            }
        }
    }

    void LateUpdate() {
        if (phoneObject == null || !phoneObject.activeSelf) return;
        if (cameraTransform == null) return;

        Vector3 targetPosition = cameraTransform.position + cameraTransform.rotation * offset;
        Quaternion targetRotation = cameraTransform.rotation * Quaternion.Euler(rotationOffset);

        if (!isAnimating) {
            phoneObject.transform.position = targetPosition;
            phoneObject.transform.rotation = targetRotation;
        }
    }

    IEnumerator AnimateShow() {
        isAnimating = true;
        phoneObject.SetActive(true);

        if (audioSource != null && openSound != null) {
            audioSource.PlayOneShot(openSound);
        }

        float elapsed = 0f;
        while (elapsed < animationDuration) {
            float t = elapsed / animationDuration;

            Vector3 targetPosition = cameraTransform.position + cameraTransform.rotation * offset;
            Vector3 startPosition = targetPosition + new Vector3(0, -0.5f, 0);

            phoneObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            phoneObject.transform.rotation = cameraTransform.rotation * Quaternion.Euler(rotationOffset);

            elapsed += Time.deltaTime;
            yield return null;
        }

        phoneObject.transform.position = cameraTransform.position + cameraTransform.rotation * offset;
        phoneObject.transform.rotation = cameraTransform.rotation * Quaternion.Euler(rotationOffset);

        isAnimating = false;
    }

    IEnumerator AnimateHide() {
        isAnimating = true;

        if (audioSource != null && closeSound != null) {
            audioSource.PlayOneShot(closeSound);
        }

        float elapsed = 0f;
        while (elapsed < animationDuration) {
            float t = elapsed / animationDuration;

            Vector3 targetPosition = cameraTransform.position + cameraTransform.rotation * offset;
            Vector3 startPosition = targetPosition;
            Vector3 endPosition = startPosition + new Vector3(0, -2f, 0);

            phoneObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            phoneObject.transform.rotation = cameraTransform.rotation * Quaternion.Euler(rotationOffset);

            elapsed += Time.deltaTime;
            yield return null;
        }

        phoneObject.SetActive(false);
        isAnimating = false;
    }
}
