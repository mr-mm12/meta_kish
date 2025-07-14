using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControll_1 : MonoBehaviour
{
    // Called when the start button is clicked
    public void OnStartButtonClicked()
    {
        // Load the "LoginManager" scene in single mode (replaces current scene)
        SceneManager.LoadScene("LoginManager", LoadSceneMode.Single);
    }
}
