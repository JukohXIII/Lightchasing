using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigidBody;

    [SerializeField] private float rotationSpeedMultiplier = 50f;
    [SerializeField] private float angularDamping = 5f;

    [Header("Inertia Settings")]
    [SerializeField] private float accelerationRate = 20f; // Taux rapide pour le démarrage (ancien 20f)
    [SerializeField] private float decelerationRate = 3f;
    [SerializeField] private float maxRotationAngle = 45f;

    // --- Private Input Variable ---
    // Stores the current input value for rotation (-1 for left, 1 for right).
    private float rotationInputValue;
    private float currentAngularVelocity;
    void Start()
    {
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }
        
        rigidBody.angularDamping = angularDamping; 
        rigidBody.gravityScale = 0;
    }

    void FixedUpdate()
{
    // Lecture de la vélocité angulaire actuelle du Rigidbody
    float currentAngularVelocity = rigidBody.angularVelocity; 
    
    // Définir la Vitesse Angulaire Cible
    float targetAngularVelocity = -rotationInputValue * rotationSpeedMultiplier;
    
    // Lecture et normalisation de l'angle Z actuel [0, 360] -> [-180, 180]
    float currentAngleZ = transform.localEulerAngles.z;
    if (currentAngleZ > 180f)
    {
        currentAngleZ -= 360f;
    }

    // --- 1. Empêcher la Vitesse Cible de Pousser au-delà de la Limite (CORRECTION) ---
    
    // Si nous sommes à la limite gauche ET que l'input pousse à gauche (négatif)
    if (currentAngleZ <= -maxRotationAngle && rotationInputValue < 0) 
    {
        targetAngularVelocity = 0f; // La cible est l'arrêt
    }
    // OU Si nous sommes à la limite droite ET que l'input pousse à droite (positif)
    else if (currentAngleZ >= maxRotationAngle && rotationInputValue > 0) 
    {
        targetAngularVelocity = 0f; // La cible est l'arrêt
    }
    
    // --- 2. Application de la Vélocité ---

    if (rotationInputValue != 0f)
    {
        // Accélération : Poussée vers la cible (qui peut être 0 si bloquée)
        rigidBody.angularVelocity = Mathf.Lerp(
            currentAngularVelocity,
            targetAngularVelocity,
            Time.fixedDeltaTime * accelerationRate
        );
    }
    else
    {
        // Décélération : Inertie de Fin (Poussée vers 0)
        rigidBody.angularVelocity = Mathf.Lerp(
            currentAngularVelocity,
            0f,
            Time.fixedDeltaTime * decelerationRate
        );
    }

    // --- 3. Contrainte Finale (Clamping) ---
    
    float clampedAngleZ = Mathf.Clamp(
        currentAngleZ, 
        -maxRotationAngle, 
        maxRotationAngle
    );
    
    // Si l'angle a été corrigé (c'est-à-dire qu'il a dépassé la limite, même légèrement)
    if (currentAngleZ != clampedAngleZ)
    {
        // Forcer la correction visuelle et annuler toute vélocité résiduelle
        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x, 
            transform.localEulerAngles.y, 
            clampedAngleZ
        );
        
        rigidBody.angularVelocity = 0f; 
    }
}

    public void OnRotation(InputAction.CallbackContext context)
    {
        // Reads the float value from the 1D Axis (A/D composite or Left Stick X-axis).
        rotationInputValue = context.ReadValue<float>();
    }
}