using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;

    // rotation control (degrees per second)
    [SerializeField] private float rotationSpeed = 180f;
    // max tilt angle (degrees) from horizontal
    [SerializeField] private float maxRotationAngle = 45f;

    // buoyancy: positive -> up (vers la surface), negative -> down
    [SerializeField] private float buoyancyTargetVelocity = 1.5f;
    // how quickly vertical velocity approaches target (higher = snappier)
    [SerializeField] private float buoyancySmoothing = 8f;

    void Start()
    {
        if (rigidBody == null) rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // lecture des touches (AZERTY: Q / D)
        // on lit les inputs en Update mais on applique en FixedUpdate pour la physique
    }

    void FixedUpdate()
    {
        if (rigidBody == null) return;

        // --- Rotation via Q / D ---
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rotationInput += 1f; // tourner à gauche
        if (Input.GetKey(KeyCode.D)) rotationInput -= 1f; // tourner à droite

        float newRotation = rigidBody.rotation + rotationInput * rotationSpeed * Time.fixedDeltaTime;
        // clamp pour éviter des rotations trop penchées
        newRotation = Mathf.Clamp(newRotation, -maxRotationAngle, maxRotationAngle);
        rigidBody.MoveRotation(newRotation);

        // --- Buoyancy (flottaison) verticale passive ---
        float currentY = rigidBody.linearVelocityY;
        float targetY = buoyancyTargetVelocity;
        float smoothedY = Mathf.Lerp(currentY, targetY, Mathf.Clamp01(buoyancySmoothing * Time.fixedDeltaTime));
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, smoothedY);
    }
    
}