using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControll_5 : MonoBehaviour
{
    // List of scene names to close when this script runs
    public string[] scenesToClose = { "LoginManager", "cutscene", "cutscene_2", "loading_screen" };

    private void OnEnable()
    {
        // Check if the currently active scene is "charecter"
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "charecter")
        {
            // Start unloading old scenes
            CloseOldScenes();
        }
    }

    // Unload the specified scenes asynchronously if they are loaded
    private void CloseOldScenes()
    {
        foreach (string sceneName in scenesToClose)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
                Debug.Log($"Scene {sceneName} is unloading.");
            }
        }
    }
}
