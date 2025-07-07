using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonBlinkEffect : MonoBehaviour
{
    [System.Serializable]
    public class BlinkButton
    {
        public Button button;  // Reference to the UI Button to blink
    }

    public List<BlinkButton> buttons;    // List of buttons to apply blink effect
    public float duration = 5f;           // Duration for one color transition cycle

    public AudioClip clickSound;          // Click sound clip set via Inspector
    private AudioSource audioSource;      // AudioSource component for playing click sounds

    // Two colors to alternate between for blinking effect
    private Color colorA = new Color32(0xF4, 0xD0, 0x3F, 0xFF); // #F4D03F
    private Color colorB = new Color32(0xF4, 0xA2, 0x3F, 0xFF); // #F4A23F

    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Initialize blinking and click sound listeners for each button
        foreach (var b in buttons)
        {
            if (b.button != null)
            {
                // Start blinking only if the button's initial color matches colorA
                if (ColorsAreEqual(b.button.image.color, colorA))
                {
                    StartCoroutine(Blink(b.button.image));
                }
                // Add click listener to play click sound on button press
                b.button.onClick.AddListener(() => PlayClickSound());
            }
        }
    }

    // Coroutine to continuously blink between colorA and colorB
    IEnumerator Blink(Image img)
    {
        while (true)
        {
            yield return StartCoroutine(LerpColor(img, colorA, colorB));
            yield return StartCoroutine(LerpColor(img, colorB, colorA));
        }
    }

    // Coroutine to smoothly interpolate the button image color between two colors
    IEnumerator LerpColor(Image img, Color from, Color to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            img.color = Color.Lerp(from, to, t);
            yield return null;
        }
    }

    // Helper method to compare two colors approximately (ignores alpha)
    bool ColorsAreEqual(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b);
    }

    // Play the click sound if AudioSource and clip are available
    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
