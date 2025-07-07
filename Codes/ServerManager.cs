using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class ServerManager : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject SigninCanvas;
    public GameObject SignupCanvas;

    [Header("Signin Fields")]
    public TMP_InputField signinUsernameInput;
    public TMP_InputField signinPasswordInput;
    public TMP_Text signinErrorText;

    [Header("Signup Fields")]
    public TMP_InputField signupUsernameInput;
    public TMP_InputField signupEmailInput;
    public TMP_InputField signupPasswordInput;

    public TMP_Text signupErrorText1;
    public TMP_Text signupErrorText2;
    public TMP_Text signupErrorText3;
    public TMP_Text signupErrorText4;

    public float emailCheckInterval = 1f;

    [Header("Server")]
    public bool useServer = false;
    public string serverURL = "http://192.168.1.102:5000";

    private string loadingSceneName = "loading_screen";
    private string finalSceneAfterLogin = "charecter";
    private string finalSceneAfterRegister = "cutscene";

    public static string nextSceneToLoad = "";

    private string localFilePath => Path.Combine(Application.persistentDataPath, "DPM.json");

    private List<OfflineUser> offlineUsers = new List<OfflineUser>();

    void Start()
    {
        SigninCanvas.SetActive(true);
        SignupCanvas.SetActive(false);

        signinErrorText.gameObject.SetActive(false);
        signupErrorText1.gameObject.SetActive(false);
        signupErrorText2.gameObject.SetActive(false);
        signupErrorText3.gameObject.SetActive(false);
        signupErrorText4.gameObject.SetActive(false);

        signupUsernameInput.onValueChanged.AddListener(OnNameChanged);
        InvokeRepeating(nameof(CheckEmailValidity), emailCheckInterval, emailCheckInterval);

        LoadOfflineUsers();
    }

    void OnDestroy()
    {
        signupUsernameInput.onValueChanged.RemoveListener(OnNameChanged);
        CancelInvoke(nameof(CheckEmailValidity));
    }

    // ========== Signin ==========
    public void OnLoginClicked()
    {
        signinErrorText.gameObject.SetActive(false);

        if (string.IsNullOrWhiteSpace(signinUsernameInput.text) ||
            string.IsNullOrWhiteSpace(signinPasswordInput.text))
        {
            signinErrorText.text = "Please fill in all fields.";
            signinErrorText.gameObject.SetActive(true);
            return;
        }

        if (useServer)
            StartCoroutine(LoginUser());
        else
            OfflineLogin();
    }

    void OfflineLogin()
    {
        foreach (var user in offlineUsers)
        {
            if (user.username == signinUsernameInput.text && user.password == signinPasswordInput.text)
            {
                Debug.Log("Offline login successful");
                nextSceneToLoad = finalSceneAfterLogin;
                SceneManager.LoadScene(loadingSceneName);
                return;
            }
        }

        signinErrorText.text = "Incorrect username or password.";
        signinErrorText.gameObject.SetActive(true);
    }

    IEnumerator LoginUser()
    {
        var data = new LoginData(signinUsernameInput.text, signinPasswordInput.text);
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(serverURL + "/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Login request failed: " + request.error);
                signinErrorText.text = "The username or password is incorrect.";
                signinErrorText.gameObject.SetActive(true);
            }
            else
            {
                var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                if (response.status == "success")
                {
                    Debug.Log("Login successful, loading cutscene...");
                    nextSceneToLoad = finalSceneAfterLogin;
                    SceneManager.LoadScene(loadingSceneName);
                }
                else
                {
                    signinErrorText.text = response.message;
                    signinErrorText.gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnHereClicked()
    {
        SigninCanvas.SetActive(false);
        SignupCanvas.SetActive(true);
    }

    // ========== Signup ==========
    public void OnRegisterClicked()
    {
        signupErrorText1.gameObject.SetActive(false);
        signupErrorText2.gameObject.SetActive(false);
        signupErrorText3.gameObject.SetActive(false);
        signupErrorText4.gameObject.SetActive(false);

        if (string.IsNullOrWhiteSpace(signupUsernameInput.text) ||
            string.IsNullOrWhiteSpace(signupEmailInput.text) ||
            string.IsNullOrWhiteSpace(signupPasswordInput.text))
        {
            signupErrorText1.text = "Please fill in all fields.";
            signupErrorText1.gameObject.SetActive(true);
            return;
        }

        if (!IsValidEmail(signupEmailInput.text))
        {
            signupErrorText2.text = "Invalid email address.";
            signupErrorText2.gameObject.SetActive(true);
            return;
        }

        if (!IsEnglishOnly(signupUsernameInput.text))
        {
            signupErrorText1.text = "Please use English characters only.";
            signupErrorText1.gameObject.SetActive(true);
            return;
        }

        if (useServer)
            StartCoroutine(RegisterUser());
        else
            OfflineRegister();
    }

    void OfflineRegister()
    {
        foreach (var user in offlineUsers)
        {
            if (user.username == signupUsernameInput.text)
            {
                signupErrorText3.text = "Username already exists.";
                signupErrorText3.gameObject.SetActive(true);
                return;
            }
            if (user.email == signupEmailInput.text)
            {
                signupErrorText4.text = "Email already exists.";
                signupErrorText4.gameObject.SetActive(true);
                return;
            }
        }

        OfflineUser newUser = new OfflineUser()
        {
            username = signupUsernameInput.text,
            email = signupEmailInput.text,
            password = signupPasswordInput.text
        };

        offlineUsers.Add(newUser);
        SaveOfflineUsers();

        Debug.Log("Offline registration successful");
        nextSceneToLoad = finalSceneAfterRegister;
        SceneManager.LoadScene(loadingSceneName);
    }

    IEnumerator RegisterUser()
    {
        var data = new RegisterData(signupUsernameInput.text, signupEmailInput.text, signupPasswordInput.text);
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(serverURL + "/register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                signupErrorText1.text = "Error communicating with server!";
                signupErrorText1.gameObject.SetActive(true);
            }
            else
            {
                var response = JsonUtility.FromJson<GenericResponse>(request.downloadHandler.text);
                if (response.status == "success")
                {
                    Debug.Log("Registration successful, loading cutscene...");
                    nextSceneToLoad = finalSceneAfterRegister;
                    SceneManager.LoadScene(loadingSceneName);
                }
                else
                {
                    signupErrorText1.text = response.message;
                    signupErrorText1.gameObject.SetActive(true);
                }
            }
        }
    }

    // ========== Offline Data ==========
    [System.Serializable]
    public class OfflineUser
    {
        public string username;
        public string email;
        public string password;
    }

    [System.Serializable]
    public class OfflineUserList
    {
        public List<OfflineUser> users;
    }

    void LoadOfflineUsers()
    {
        if (File.Exists(localFilePath))
        {
            string json = File.ReadAllText(localFilePath);
            OfflineUserList list = JsonUtility.FromJson<OfflineUserList>(json);
            if (list != null)
                offlineUsers = list.users;
        }
    }

    void SaveOfflineUsers()
    {
        OfflineUserList list = new OfflineUserList { users = offlineUsers };
        string json = JsonUtility.ToJson(list, true);
        File.WriteAllText(localFilePath, json);
    }

    // ========== Realtime Validation ==========
    void OnNameChanged(string newText)
    {
        if (!IsEnglishOnly(newText))
        {
            signupErrorText1.text = "Please use English characters only.";
            signupErrorText1.gameObject.SetActive(true);
        }
        else
        {
            signupErrorText1.gameObject.SetActive(false);
        }
    }

    void CheckEmailValidity()
    {
        string email = signupEmailInput.text;
        if (string.IsNullOrWhiteSpace(email))
        {
            signupErrorText2.gameObject.SetActive(false);
            return;
        }

        if (!IsValidEmail(email))
        {
            signupErrorText2.text = "Invalid email address.";
            signupErrorText2.gameObject.SetActive(true);
        }
        else
        {
            signupErrorText2.gameObject.SetActive(false);
        }
    }

    bool IsEnglishOnly(string text)
    {
        return Regex.IsMatch(text, "^[a-zA-Z0-9\\s.,!?@#_\\-]*$");
    }

    bool IsValidEmail(string email)
    {
        string pattern = "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    // ========== Data Classes ==========
    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
        public LoginData(string u, string p) { username = u; password = p; }
    }

    [System.Serializable]
    public class RegisterData
    {
        public string username;
        public string email;
        public string password;
        public RegisterData(string u, string e, string p)
        { username = u; email = e; password = p; }
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string status;
        public string message;
    }

    [System.Serializable]
    public class GenericResponse
    {
        public string status;
        public string message;
    }
}