using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PressureController : MonoBehaviour
{
    [Header("Pressure Settings")]
    public float maxPressure = 100f;
    public float pressureIncreaseRate = 0.5f;  // rate per second when ascending too fast
    public float pressureDecreaseRate = 0.2f;  // rate per second when safe or descending
    public float safeAscendSpeed = 2f;         // max safe upward speed (units per second)
    public float maxDepth = 8000f;

    [Header("References")]
    public Rigidbody2D fishRigidbody;

    [Header("Runtime Variables")]
    public float currentPressure = 0f;
    public float currentDepth = 0f; // Positive number for depth

    public UnityEvent OnPressureMaxReached; // Assign death event in inspector

    void Start()
    {
        if (fishRigidbody == null)
            fishRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Update depth: assuming surface at y=0, depth is positive downward
        currentDepth = Mathf.Clamp(transform.position.y, 0f, maxDepth);

        float verticalSpeed = fishRigidbody.linearVelocityY;

        if (verticalSpeed > safeAscendSpeed)
        {
            // Ascending too fast, increase pressure
            currentPressure += pressureIncreaseRate * Time.deltaTime;
        }
        else
        {
            // Descending or safe speed, decrease pressure
            currentPressure -= pressureDecreaseRate * Time.deltaTime;
        }

        currentPressure = Mathf.Clamp(currentPressure, 0f, maxPressure);

        if (currentPressure >= maxPressure)
        {
            Debug.Log("Pressure Max Reached! Player dies.");
            OnPressureMaxReached?.Invoke();
        }
    }

    // Optional: UI getter
    public float GetPressurePercent()
    {
        return currentPressure / maxPressure;
    }

    public float GetDepth()
    {
        return currentDepth;
    }
}
