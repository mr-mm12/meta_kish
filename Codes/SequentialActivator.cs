using UnityEngine;

public class SequentialActivator : MonoBehaviour
{
    public Transform playerTransform;        // بازیکن
    public GameObject[] objects;             // آبجکت‌هایی که باید یکی‌یکی فعال بشن
    public float triggerDistance = 30f;      // فاصله فعال‌سازی بعدی

    private int currentIndex = 0;
    private bool isWaitingForNext = false;

    void Start()
    {
        // همه غیر فعال، فقط اولی فعال
        for (int i = 0; i < objects.Length; i++)
            objects[i].SetActive(i == 0);
    }

    void Update()
    {
        if (currentIndex >= objects.Length) return;

        float distance = Vector3.Distance(playerTransform.position, objects[currentIndex].transform.position);

        if (distance <= triggerDistance && !isWaitingForNext)
        {
            isWaitingForNext = true;
            StartCoroutine(ActivateNextAfterDelay(0.3f)); // با تأخیر خیلی کم برای نظم
        }
    }

    System.Collections.IEnumerator ActivateNextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // غیرفعال کردن فعلی
        objects[currentIndex].SetActive(false);

        currentIndex++;
        isWaitingForNext = false;

        // فعال‌کردن بعدی (اگر باقی مونده)
        if (currentIndex < objects.Length)
            objects[currentIndex].SetActive(true);
    }
}
