using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControll_4 : MonoBehaviour
{
    void Start()
    {
        // Start coroutine to switch to "charecter" scene after a delay
        StartCoroutine(SwitchSceneAfterDelay(4));
    }

    // Coroutine that waits for specified seconds, then loads "charecter" scene
    IEnumerator SwitchSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("charecter");
    }
}
