using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public AudioClip backgroundMusic;   // The audio clip to play as background music
    private AudioSource audioSource;    // AudioSource component to play the clip

    void Start()
    {
        // Try to get AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Assign the background music clip and configure looping
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Play music only if the active scene is "Main_Menu"
        if (SceneManager.GetActiveScene().name == "Main_Menu")
        {
            if (backgroundMusic != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
