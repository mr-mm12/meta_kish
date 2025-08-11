using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SunController : MonoBehaviour
{
    [Header("Duration Settings")]
    public float dayDuration = 864f;

    [Header("Sun Settings")]
    public Transform sunTransform;
    public Light sunLight; // نور خورشید
    public Light moonLight; // نور ماه

    [Header("Volume Profiles")]
    public VolumeProfile dayProfile;
    public VolumeProfile nightProfile;
    public Volume volumeComponent; // Volume اصلی صحنه

    [Header("Manual Control")]
    public bool manualControl = false;

    [Range(0f, 2f)]
    public float dayProgress = 0f;

    private bool lastStateIsDay = true;

    void Start()
    {
        dayProgress = 0f;
        volumeComponent.profile = dayProfile;
        sunLight.gameObject.SetActive(true);
        moonLight.gameObject.SetActive(false);
        lastStateIsDay = true;
    }

    void Update()
    {
        if (sunTransform == null || sunLight == null || moonLight == null || volumeComponent == null) return;

        if (!manualControl)
            dayProgress = (Time.time % dayDuration) / (dayDuration / 2f); 
            // dayProgress بین 0 تا 2

        UpdateSun(dayProgress < 1f ? dayProgress : dayProgress - 1f); 
        UpdateMoonRotation();

        CheckDayNightSwitch();
    }

    void UpdateSun(float time)
    {
        // time الان از 0 تا 1 هست، برای کل روز (صبح یا شب)
        float sunY = Mathf.Lerp(180f, 0f, time);
        float sunX = time < 0.5f
            ? Mathf.Lerp(185f, 90f, time * 2)
            : Mathf.Lerp(90f, 185f, (time - 0.5f) * 2);

        sunTransform.rotation = Quaternion.Euler(sunX, sunY, 0f);

        float intensity = time < 0.5f
            ? Mathf.Lerp(40000f, 80000f, time * 2)
            : Mathf.Lerp(80000f, 40000f, (time - 0.5f) * 2);

        sunLight.intensity = intensity;
    }

    void UpdateMoonRotation()
    {
        if (dayProgress >= 1f && dayProgress <= 2f)
        {
            float nightProgress = dayProgress - 1f; // 0 تا 1

            float x = Mathf.Lerp(0f, 180f, nightProgress);

            float y = (nightProgress <= 0.5f) 
                ? Mathf.Lerp(0f, -90f, nightProgress * 2f)      
                : Mathf.Lerp(-90f, 0f, (nightProgress - 0.5f) * 2f); 

            moonLight.transform.rotation = Quaternion.Euler(x, y, 0f);
        }
    }

    void CheckDayNightSwitch()
    {
        if (dayProgress < 1f && !lastStateIsDay)
        {
            volumeComponent.profile = dayProfile;
            sunLight.gameObject.SetActive(true);
            moonLight.gameObject.SetActive(false);
            lastStateIsDay = true;
        }
        else if (dayProgress >= 1f && lastStateIsDay)
        {
            volumeComponent.profile = nightProfile;
            sunLight.gameObject.SetActive(false);
            moonLight.gameObject.SetActive(true);
            lastStateIsDay = false;
        }
    }
}
