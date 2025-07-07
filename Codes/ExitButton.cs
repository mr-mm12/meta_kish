using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // Method called when the exit button is clicked
    public void OnExitClicked()
    {
        // If running inside the Unity Editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a built application, quit the application completely
        Application.Quit();
#endif
    }
}
