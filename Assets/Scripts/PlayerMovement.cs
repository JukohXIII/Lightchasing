using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float rotationSpeedMultiplier = 50f;
    [SerializeField] private float angularDamping = 5f;

    [Header("Inertia Settings")]
    [SerializeField] private float accelerationRate = 200f;
    [SerializeField] private float decelerationRate = 200f;
    [SerializeField] private float maxRotationAngle = 45f;

    private float rotationInputValue;
    private float currentAngularVelocity;
    void Start()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        rigidBody.angularDamping = angularDamping;
        rigidBody.gravityScale = 0f;
    }

    private float NormalizeAngle(float a)
    {
        if (a > 180f) a -= 360f;
        return a;
    }

    void FixedUpdate()
    {
        currentAngularVelocity = rigidBody.angularVelocity;

        float currentAngleZ = NormalizeAngle(transform.localEulerAngles.z);

        float desiredAngularVelocity = -rotationInputValue * rotationSpeedMultiplier;

        bool atLeftLimit  = currentAngleZ <= -maxRotationAngle;
        bool atRightLimit = currentAngleZ >=  maxRotationAngle;

        if (atLeftLimit && desiredAngularVelocity < 0f)
        {
            desiredAngularVelocity = 0f;
        }
        else if (atRightLimit && desiredAngularVelocity > 0f)
        {
            desiredAngularVelocity = 0f;
        }

        if (Mathf.Approximately(rotationInputValue, 0f))
            desiredAngularVelocity = 0f;

        float accel = (Mathf.Abs(desiredAngularVelocity) > Mathf.Abs(currentAngularVelocity)) ? accelerationRate : decelerationRate;
        float newAngularVelocity = Mathf.MoveTowards(currentAngularVelocity, desiredAngularVelocity, accel * Time.fixedDeltaTime);

        rigidBody.angularVelocity = newAngularVelocity;

        float clampedAngleZ = Mathf.Clamp(currentAngleZ, -maxRotationAngle, maxRotationAngle);
        if (!Mathf.Approximately(currentAngleZ, clampedAngleZ))
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, clampedAngleZ);
            if ((currentAngleZ < -maxRotationAngle && rotationInputValue < 0f) ||
                (currentAngleZ > maxRotationAngle && rotationInputValue > 0f))
            {
                rigidBody.angularVelocity = 0f;
            }
        }
        
        spriteRenderer.flipX = currentAngleZ < 0f;
    }

    public void OnRotation(InputAction.CallbackContext context)
    {
        rotationInputValue = context.ReadValue<float>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            FindAnyObjectByType<GameManager>().TakeDamage();
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.CompareTag("Finish"))
        {
            FindAnyObjectByType<GameManager>().HandleVictory();
        }
    }
}
