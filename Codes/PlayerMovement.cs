using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float walkAccelerationTime = 0.05f;
    public float runAccelerationTime = 0.4f;
    public Camera cameraRef;

    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private float accelerationTime = 0.1f;
    private Vector3 moveDirection = Vector3.zero;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // ✅ جلوگیری از عبور از اجسام
        rb.interpolation = RigidbodyInterpolation.Interpolate;         // ✅ حرکت نرم‌تر
        rb.constraints = RigidbodyConstraints.FreezeRotation;          // ✅ جلوگیری از چرخش ناخواسته
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cameraRef.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraRef.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 inputDirection = camForward * moveZ + camRight * moveX;

        if (inputDirection.magnitude > 0.01f)
            moveDirection = inputDirection.normalized;
        else
            moveDirection = Vector3.zero;

        bool isWalking = moveDirection.magnitude > 0 && !Input.GetKey(KeyCode.X);
        bool isRunning = moveDirection.magnitude > 0 && Input.GetKey(KeyCode.X);

        if (isRunning)
        {
            targetSpeed = runSpeed;
            accelerationTime = Mathf.Max(runAccelerationTime, 0.01f);
        }
        else if (isWalking)
        {
            targetSpeed = walkSpeed;
            accelerationTime = Mathf.Max(walkAccelerationTime, 0.01f);
        }
        else
        {
            targetSpeed = 0f;
            accelerationTime = 0.1f;
        }
    }

    void FixedUpdate()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, (1f / accelerationTime) * Time.fixedDeltaTime * targetSpeed);
        Vector3 movement = moveDirection * currentSpeed;

        // ✅ بررسی برخورد قبل از حرکت برای جلوگیری از ورود به داخل آبجکت‌ها
        if (movement.magnitude > 0.01f)
        {
            RaycastHit hit;
            if (!Physics.Raycast(rb.position, movement.normalized, out hit, currentSpeed * Time.fixedDeltaTime + 0.1f))
            {
                rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
            }
            else
            {
                // اگر به مانعی برخورد کرد، سرعت رو صفر کن
                currentSpeed = 0f;
            }
        }
    }
}
