using UnityEngine;

public class PhoneToggle : MonoBehaviour {
    public GameObject phoneObject;
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            phoneObject.SetActive(!phoneObject.activeSelf);
        }
    }
}
