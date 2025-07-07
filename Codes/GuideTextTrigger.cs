using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GuideTextTrigger : MonoBehaviour
{
    [Header("Text UI")]
    public TextMeshProUGUI guideText;        // UI text component to show the guide
    public GameObject okButton;              // Button to continue after the text is shown

    [Header("Player & Trigger Zone")]
    public Transform playerTransform;        // Reference to the player transform
    public Transform guideZone;              // Trigger zone to start the guide
    public MonoBehaviour playerControllerScript;   // Script controlling the player
    public MonoBehaviour cameraControllerScript;   // Script controlling the camera

    [Header("Objects to Activate")]
    public GameObject[] objectsToActivate;   // Objects that should activate when the guide starts

    [Header("Typing Settings")]
    public float triggerDistance = 5f;       // Distance from trigger to player to start the guide
    public float typingSpeed = 0.05f;        // Speed of typing effect for the text
    public AudioClip typingClip;             // Audio clip to play during typing

    private AudioSource internalAudioSource; // Internal audio source for playing typing sound
    private bool hasPlayed = false;          // To ensure the guide plays only once

    [TextArea]
    [Tooltip("You can use rich tags here, like <b>, <i>, <color=red>, <size=150%> etc.")]
    public string fullText = 
        "Welcome to <color=#5B34CE><b>MetaKish</b></color>!\n\n" +
        "This game is <b><color=#4CAF50>based on the real Kish Island</color></b>, with its beauty and style carefully recreated.\n" +
        "We hope you enjoy exploring and playing.\n\n" +
        "<b><color=#F3A346>Note: In some parts of the game, you will see a cube with an arrow. (If you see it, <color=#FF0005>follow it!</color>)</color></b>\n\n" +
        "Press the <b>'OK'</b> button to start.";

    void Start()
    {
        // Enable rich text formatting in TextMeshPro
        if (guideText != null)
            guideText.richText = true;

        // Setup internal audio source for typing sound
        if (typingClip != null)
        {
            internalAudioSource = gameObject.AddComponent<AudioSource>();
            internalAudioSource.clip = typingClip;
            internalAudioSource.playOnAwake = false;
            internalAudioSource.volume = 0.2f;
        }

        // Disable all objects that will be activated later
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Hide OK button initially
        okButton.SetActive(false);

        // Start checking player distance to the trigger zone
        StartCoroutine(CheckDistanceCoroutine());
    }

    IEnumerator CheckDistanceCoroutine()
    {
        // Continuously check if the player is within the trigger distance
        while (!hasPlayed)
        {
            float sqrDistance = (playerTransform.position - guideZone.position).sqrMagnitude;
            if (sqrDistance <= triggerDistance * triggerDistance)
            {
                hasPlayed = true;

                // Activate all designated objects
                foreach (GameObject obj in objectsToActivate)
                {
                    if (obj != null)
                        obj.SetActive(true);
                }

                // Disable player and camera controls
                if (playerControllerScript != null)
                    playerControllerScript.enabled = false;

                if (cameraControllerScript != null)
                    cameraControllerScript.enabled = false;

                // Show and unlock the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // Start typing the text
                StartCoroutine(ShowText());
                yield break;
            }

            // Wait before checking again
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ShowText()
    {
        guideText.text = "";

        // Start typing sound
        if (internalAudioSource != null)
            internalAudioSource.Play();

        // Use rich text-aware typing by parsing tags
        int i = 0;
        while (i < fullText.Length)
        {
            // If we encounter a tag, add the entire tag at once
            if (fullText[i] == '<')
            {
                int closeTag = fullText.IndexOf('>', i);
                if (closeTag != -1)
                {
                    string tag = fullText.Substring(i, closeTag - i + 1);
                    guideText.text += tag;
                    i = closeTag + 1;
                    continue;
                }
            }

            // Add one character at a time
            guideText.text += fullText[i];
            i++;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Stop typing sound after completion
        if (internalAudioSource != null)
            internalAudioSource.Stop();

        // Show OK button
        okButton.SetActive(true);
    }

    public void OnOkClicked()
    {
        // Clear the guide text and hide the OK button
        guideText.text = "";
        okButton.SetActive(false);

        // Deactivate the guide objects
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Re-enable player and camera controls
        if (playerControllerScript != null)
            playerControllerScript.enabled = true;

        if (cameraControllerScript != null)
            cameraControllerScript.enabled = true;

        // Lock and hide the cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
