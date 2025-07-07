using UnityEngine;

public class FloatingRotator : MonoBehaviour
{
    public float rotationSpeed = 50f;      // سرعت چرخش
    public float floatAmplitude = 10f;     // مقدار بالا و پایین رفتن
    public float floatFrequency = 1f;      // سرعت بالا و پایین رفتن

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // چرخش مداوم دور محور Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // بالا و پایین رفتن به صورت سینوسی
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
