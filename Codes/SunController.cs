using UnityEngine;

public class SunController : MonoBehaviour
{
    [Header("Duration Settings")]
    public float dayDuration = 864f;

    [Header("Sun Settings")]
    public Transform sunTransform;
    public Light sunLight;

    [Header("Manual Control")]
    public bool manualControl = false; // 🔁 اگر فعال باشه، کاربر کنترل رو در دست داره

    [Range(0f, 1f)]
    public float dayProgress = 0f; // 🌅 پیشرفت روز از صبح تا شب

    void Update()
    {
        if (sunTransform == null || sunLight == null) return;

        // ⏳ اگر کنترل دستی غیرفعاله، زمان بر اساس گذر زمان بازی محاسبه میشه
        if (!manualControl)
            dayProgress = (Time.time % dayDuration) / dayDuration;

        // 🎯 محاسبه زاویه و شدت نور
        UpdateSun(dayProgress);
    }

    void UpdateSun(float time)
    {
        // 🌞 زاویه افقی (Y)
        float sunY = Mathf.Lerp(180f, 0f, time);

        // 🌞 زاویه عمودی (X)
        float sunX = time < 0.5f
            ? Mathf.Lerp(180f, 90f, time * 2)
            : Mathf.Lerp(90f, 180f, (time - 0.5f) * 2);

        sunTransform.rotation = Quaternion.Euler(sunX, sunY, 0f);

        // 💡 شدت نور
        float intensity = time < 0.5f
            ? Mathf.Lerp(40000f, 80000f, time * 2)
            : Mathf.Lerp(80000f, 40000f, (time - 0.5f) * 2);

        sunLight.intensity = intensity;
    }
}
