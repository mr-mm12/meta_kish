using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControll_3 : MonoBehaviour
{
    void Start()
    {
        // Start coroutine to switch scene after a delay of 17 seconds
        StartCoroutine(SwitchSceneAfterDelay(17));
    }

    // Coroutine to wait for specified delay, then load "cutscene_2" scene
    IEnumerator SwitchSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("cutscene_2");
    }
}
