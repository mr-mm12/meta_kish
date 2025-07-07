using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GuideTextTrigger : MonoBehaviour
{
    [Header("Text UI")]
    public TextMeshProUGUI guideText;
    public GameObject okButton;

    [Header("Player & Trigger Zone")]
    public Transform playerTransform;
    public Transform guideZone;
    public MonoBehaviour playerControllerScript;
    public MonoBehaviour cameraControllerScript;

    [Header("Objects to Activate")]
    public GameObject[] objectsToActivate;

    [Header("Typing Settings")]
    public float triggerDistance = 5f;
    public float typingSpeed = 0.05f;
    public AudioClip typingClip;

    private AudioSource internalAudioSource;
    private bool hasPlayed = false;

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
        // اطمینان از فعال بودن Rich Text
        if (guideText != null)
            guideText.richText = true;

        // ساخت منبع صدا
        if (typingClip != null)
        {
            internalAudioSource = gameObject.AddComponent<AudioSource>();
            internalAudioSource.clip = typingClip;
            internalAudioSource.playOnAwake = false;
            internalAudioSource.volume = 0.2f;
        }

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        okButton.SetActive(false);
        StartCoroutine(CheckDistanceCoroutine());
    }

    IEnumerator CheckDistanceCoroutine()
    {
        while (!hasPlayed)
        {
            float sqrDistance = (playerTransform.position - guideZone.position).sqrMagnitude;
            if (sqrDistance <= triggerDistance * triggerDistance)
            {
                hasPlayed = true;

                foreach (GameObject obj in objectsToActivate)
                {
                    if (obj != null)
                        obj.SetActive(true);
                }

                if (playerControllerScript != null)
                    playerControllerScript.enabled = false;

                if (cameraControllerScript != null)
                    cameraControllerScript.enabled = false;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                StartCoroutine(ShowText());
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ShowText()
    {
        guideText.text = "";

        if (internalAudioSource != null)
            internalAudioSource.Play();

        // چون تگ‌های TMP چند کاراکتری هستن، نباید حروف رو یکی‌یکی بنویسیم.
        // پس باید از این روش استفاده کنیم که کل تگ رو حفظ کنه:
        int i = 0;
        while (i < fullText.Length)
        {
            // اگر کاراکتر '<' دیدی، یعنی تگ شروع شده
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

            guideText.text += fullText[i];
            i++;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (internalAudioSource != null)
            internalAudioSource.Stop();

        okButton.SetActive(true);
    }

    public void OnOkClicked()
    {
        guideText.text = "";
        okButton.SetActive(false);

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (playerControllerScript != null)
            playerControllerScript.enabled = true;

        if (cameraControllerScript != null)
            cameraControllerScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
