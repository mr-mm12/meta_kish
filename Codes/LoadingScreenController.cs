using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    [Header("Backgrounds")]
    public GameObject[] backgroundImages;  // Array of possible background images

    [Header("Loading Bar")]
    public RectTransform bar;               // UI element for the loading bar
    public Vector2 startPos = new Vector2(-70f, 0f);  // Start position of the loading bar
    public Vector2 endPos = new Vector2(-10f, 0f);    // End position of the loading bar

    private float duration;       // Total duration for the loading bar animation
    private float elapsedTime;    // Time passed since start of loading bar animation
    private bool finished = false; // Flag to check if loading is finished

    void Start()
    {
        // Randomly select one background image and activate only that one
        int randomIndex = Random.Range(0, backgroundImages.Length);
        for (int i = 0; i < backgroundImages.Length; i++)
        {
            backgroundImages[i].SetActive(i == randomIndex);
        }

        // Randomize loading duration between 5 to 10 seconds
        duration = Random.Range(5f, 10f);
        elapsedTime = 0f;

        // Set loading bar initial position
        bar.anchoredPosition = startPos;
    }

    void Update()
    {
        if (!finished)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Apply custom easing to loading progress
            float easedT = CustomEase(t);

            // Update loading bar position based on eased progress
            bar.anchoredPosition = Vector2.Lerp(startPos, endPos, easedT);

            if (elapsedTime >= duration)
            {
                finished = true;
                LoadMainScene();
            }
        }
    }

    // Custom easing function for smoother loading bar animation
    float CustomEase(float t)
    {
        if (t < 0.7f)
        {
            // Slow start for first 70% progress using sine easing
            return Mathf.Sin(t * Mathf.PI * 0.5f) * 0.7f;
        }
        else
        {
            // Accelerate during last 30% progress using quadratic easing
            float t2 = (t - 0.7f) / 0.3f; // Normalize to [0,1]
            return 0.7f + Mathf.Pow(t2, 2) * 0.3f;
        }
    }

    // Loads the main scene asynchronously and unloads loading screen when done
    void LoadMainScene()
    {
        string sceneToLoad = ServerManager.nextSceneToLoad;
        if (string.IsNullOrEmpty(sceneToLoad))
            sceneToLoad = "cutscene";

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        loadOp.completed += (AsyncOperation op) =>
        {
            Scene scene = SceneManager.GetSceneByName(sceneToLoad);
            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);
            }
            SceneManager.UnloadSceneAsync("loading_screen");
        };
    }
}
