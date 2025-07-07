using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonHandler : MonoBehaviour
{
    // Called when the login/open button is clicked
    public void OnLoginOpenClicked()
    {
        // Load the "charecter" scene
        SceneManager.LoadScene("charecter");
    }
}
