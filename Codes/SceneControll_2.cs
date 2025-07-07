using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneControll_2 : MonoBehaviour
{
    // Subscribe to sceneLoaded event when this component is enabled
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Unsubscribe from sceneLoaded event when this component is disabled
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "loading_screen")
        {
            Debug.Log("loading_screen loaded, waiting for LoadingScreenController to load next scene");
        }
    }

    // Coroutine to load the next scene after a specified delay
    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!string.IsNullOrEmpty(ServerManager.nextSceneToLoad))
            SceneManager.LoadScene(ServerManager.nextSceneToLoad);
        else
            SceneManager.LoadScene("cutscene");
    }
}
