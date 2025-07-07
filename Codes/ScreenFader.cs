using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image blackImage;          // Black overlay image on the screen
    public float sceneDuration = 4f;  // Total scene duration in seconds
    private float timer = 0f;         // Elapsed time since start
    private bool fadeInStarted = false; // Flag to check if fade-in has started

    void Start()
    {
        // Start fully black (alpha = 1)
        SetAlpha(1f);
        blackImage.gameObject.SetActive(true);

        // Start fading out (from black to transparent) during first 1 second
        StartCoroutine(FadeOut(1f));
    }

    void Update()
    {
        timer += Time.deltaTime;

        // When reaching the last 1 second of scene duration and fade-in not started yet
        if (!fadeInStarted && timer >= sceneDuration - 1f)
        {
            fadeInStarted = true;
            StartCoroutine(FadeIn(1f));
        }
    }

    IEnumerator FadeOut(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1f, 0f, elapsed / duration));
            yield return null;
        }

        SetAlpha(0f);
        // Do not deactivate the blackImage here to avoid flicker issues
    }

    IEnumerator FadeIn(float duration)
    {
        blackImage.gameObject.SetActive(true);
        SetAlpha(0f); // Ensure alpha is zero at start of fade-in

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, 1f, elapsed / duration));
            yield return null;
        }

        SetAlpha(1f);
    }

    // Helper method to set the alpha channel of the blackImage color
    void SetAlpha(float alpha)
    {
        Color color = blackImage.color;
        color.a = alpha;
        blackImage.color = color;
    }
}
