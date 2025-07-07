using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CarAutoController : MonoBehaviour
{
    [Header("Wheel Setup")]
    public WheelCollider frontLeft, frontRight, rearLeft, rearRight;   // WheelColliders for physics
    public Transform frontLeftTransform, frontRightTransform, rearLeftTransform, rearRightTransform; // Visuals

    [Header("Drive Parameters")]
    public float motorForce = 100000f;                    // Engine force
    public float brakeForce = 800000f;                    // Braking force
    public float maxSteerAngle = 30f;                     // Maximum steering angle
    public float frictionStiffnessIdle = 2f;              // Friction when idle
    public float frictionStiffnessAccelerate = 1.2f;      // Friction when accelerating

    [Header("Path Points")]
    public Transform[] pathPoints;                        // Waypoints for the car to follow
    [Tooltip("Speed (km/h) per waypoint; must match pathPoints length")]
    public float[] speedKmh;                              // Desired speed for each waypoint
    public float pointReachDistance = 100f;               // Distance threshold to switch to next waypoint

    [Header("UI & Audio")]
    public TextMeshProUGUI speedText;                     // UI Text for speed
    public AudioClip stopSound;                           // Sound when reaching the end
    public AudioClip engineLoopSound;                     // Engine loop sound
    public float minPitch = 0.8f;                         // Minimum engine pitch
    public float maxPitch = 2f;                           // Maximum engine pitch
    public float maxSpeedKmh = 100f;                      // Speed at which pitch is maxed
    [Tooltip("If enabled, shows speed UI (only when inside the car)")]
    public bool showSpeedUI = true;
    [Range(0f, 1f)] public float engineVolume = 0.2f;     // Engine audio volume

    [Header("Indicators")]
    public GameObject[] forwardObjects;                   // e.g., headlights
    public GameObject[] reverseObjects;                   // e.g., reverse lights

    [Header("UI - Progress Bar")]
    public Slider pathProgressSlider;                     // Visual progress bar for path completion

    private AudioSource audioSource;
    private Rigidbody rb;
    private CarEntryController entryController;

    private int currentPointIndex = 0;                    // Current target waypoint index
    private bool hasStoppedAtLastPoint = false;           // Whether we've stopped at the final point

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = gameObject.AddComponent<AudioSource>();
        entryController = GetComponent<CarEntryController>();
        SetFriction(frictionStiffnessIdle);

        if (speedKmh == null || speedKmh.Length != pathPoints.Length)
            Debug.LogWarning("speedKmh length must match pathPoints length!");

        ToggleObjects(forwardObjects, false);
        ToggleObjects(reverseObjects, false);

        if (pathProgressSlider != null)
            pathProgressSlider.gameObject.SetActive(false);

        if (engineLoopSound != null)
        {
            audioSource.clip = engineLoopSound;
            audioSource.loop = true;
            audioSource.volume = engineVolume;
            audioSource.playOnAwake = false;
        }
    }

    private void FixedUpdate()
    {
        if (entryController == null) return;

        UpdatePathProgressUI();

        if (!entryController.playerEnteredCar)
        {
            HideSpeed();
            if (audioSource.isPlaying) audioSource.Stop();
            ToggleObjects(forwardObjects, false);
            return;
        }

        if (!audioSource.isPlaying && engineLoopSound != null)
            audioSource.Play();

        if (pathPoints.Length == 0) return;

        DriveToWaypoint();
        UpdateWheels();
        UpdateSpeedUI();

        ToggleObjects(forwardObjects, true);
    }

    private void DriveToWaypoint()
    {
        Transform target = pathPoints[currentPointIndex];
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance < pointReachDistance * 0.9f)
        {
            currentPointIndex = (currentPointIndex + 1) % pathPoints.Length;
            target = pathPoints[currentPointIndex];
            distance = Vector3.Distance(transform.position, target.position);
        }

        Vector3 localTarget = transform.InverseTransformPoint(target.position);
        float angleToTarget = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float steerInput = Mathf.Clamp(angleToTarget / maxSteerAngle, -1f, 1f);
        ApplySteering(steerInput);

        float targetKmh = (speedKmh != null && currentPointIndex < speedKmh.Length) ? speedKmh[currentPointIndex] : 0f;
        float targetMs = targetKmh / 3.6f;
        float currentMs = rb.velocity.magnitude;
        float error = targetMs - currentMs;
        float deadZone = 0.5f;

        float input = 0f;
        if (error > deadZone) input = Mathf.Clamp(error * 0.5f, 0f, 1f);
        else if (error < -deadZone) input = Mathf.Clamp(error * 0.5f, -1f, 0f);

        bool reversing = localTarget.z < -0.1f;
        if (reversing)
        {
            ApplyReverse(Mathf.Abs(input));
            ToggleObjects(reverseObjects, true);
            ToggleObjects(forwardObjects, false);
        }
        else
        {
            ApplyForward(Mathf.Max(input, 0f));
            ToggleObjects(forwardObjects, input > 0.05f);
            ToggleObjects(reverseObjects, false);
        }

        float stiff = (!reversing && input > 0.1f) ? frictionStiffnessAccelerate : frictionStiffnessIdle;
        SetFriction(stiff);

        if (currentPointIndex == pathPoints.Length - 1 && distance < pointReachDistance)
        {
            ApplyForward(0f);
            ApplyBraking(brakeForce);
            if (!hasStoppedAtLastPoint)
            {
                hasStoppedAtLastPoint = true;
                if (stopSound != null) audioSource.PlayOneShot(stopSound);
            }
            ToggleObjects(forwardObjects, false);
            ToggleObjects(reverseObjects, false);
            if (entryController.playerEnteredCar)
                entryController.ExitCar();
            if (audioSource.isPlaying) audioSource.Stop();
            enabled = false;
        }
    }

    private void ApplyForward(float input)
    {
        SetMotorTorque(motorForce * input);
        float currentSpeed = rb.velocity.magnitude;
        float targetSpeed = speedKmh[currentPointIndex] / 3.6f;

        if (currentSpeed > targetSpeed + 0.5f)
            ApplyBraking(brakeForce * 0.3f);
        else
            ApplyBraking(input < 0.01f ? brakeForce : 0f);
    }

    private void ApplyReverse(float input)
    {
        float torque = motorForce * input;
        frontLeft.motorTorque = -torque;
        frontRight.motorTorque = -torque;
        rearLeft.motorTorque = -torque;
        rearRight.motorTorque = -torque;
        ApplyBraking(0f);
    }

    private void SetMotorTorque(float torque)
    {
        frontLeft.motorTorque = torque;
        frontRight.motorTorque = torque;
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    private void ApplyBraking(float brake)
    {
        frontLeft.brakeTorque = brake;
        frontRight.brakeTorque = brake;
        rearLeft.brakeTorque = brake;
        rearRight.brakeTorque = brake;
    }

    private void ApplySteering(float input)
    {
        float angle = maxSteerAngle * input;
        frontLeft.steerAngle = angle;
        frontRight.steerAngle = angle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeft, frontLeftTransform);
        UpdateSingleWheel(frontRight, frontRightTransform);
        UpdateSingleWheel(rearLeft, rearLeftTransform);
        UpdateSingleWheel(rearRight, rearRightTransform);
    }

    private void UpdateSingleWheel(WheelCollider col, Transform tf)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        tf.position = pos;
        tf.rotation = rot;
    }

    private void SetFriction(float s)
    {
        void F(WheelCollider w)
        {
            var f = w.forwardFriction; f.stiffness = s; w.forwardFriction = f;
            var sf = w.sidewaysFriction; sf.stiffness = s; w.sidewaysFriction = sf;
        }
        F(frontLeft); F(frontRight); F(rearLeft); F(rearRight);
    }

    private void UpdateSpeedUI()
    {
        if (speedText == null || rb == null) return;

        if (entryController.playerEnteredCar)
        {
            float kmh = rb.velocity.magnitude * 3.6f;
            if (showSpeedUI)
            {
                speedText.text = $"Speed: {kmh:0} km/h";
                speedText.gameObject.SetActive(true);
            }
            else speedText.gameObject.SetActive(false);

            if (engineLoopSound != null)
            {
                float pct = Mathf.Clamp01(kmh / maxSpeedKmh);
                audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, pct);
            }
        }
        else
        {
            HideSpeed();
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }

    private void HideSpeed()
    {
        if (speedText != null) speedText.gameObject.SetActive(false);
    }

    private void ToggleObjects(GameObject[] objs, bool on)
    {
        if (objs == null) return;
        foreach (var o in objs) if (o != null) o.SetActive(on);
    }

    private void UpdatePathProgressUI()
    {
        if (pathProgressSlider == null || pathPoints.Length < 2) return;

        if (!entryController.playerEnteredCar)
        {
            pathProgressSlider.gameObject.SetActive(false);
            return;
        }

        pathProgressSlider.gameObject.SetActive(true);

        float total = 0f, traveled = 0f;
        for (int i = 0; i < pathPoints.Length - 1; i++)
            total += Vector3.Distance(pathPoints[i].position, pathPoints[i + 1].position);
        for (int i = 0; i < currentPointIndex; i++)
            traveled += Vector3.Distance(pathPoints[i].position, pathPoints[i + 1].position);

        float seg = Vector3.Distance(pathPoints[currentPointIndex].position,
                                     pathPoints[Mathf.Min(currentPointIndex + 1, pathPoints.Length - 1)].position);
        float toNext = Vector3.Distance(transform.position,
                                        pathPoints[Mathf.Min(currentPointIndex + 1, pathPoints.Length - 1)].position);

        float localPct = seg > 0 ? (seg - toNext) / seg : 0f;
        pathProgressSlider.value = (traveled + localPct * seg) / total;
    }
}
