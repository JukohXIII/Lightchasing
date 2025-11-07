using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("Reference to the Rigidbody2D component.")]
    [SerializeField] private Rigidbody2D rigidBody;

    [Header("Rotation Settings")]
    [Tooltip("Angular speed multiplier for rotation.")]
    [SerializeField] private float rotationSpeedMultiplier = 50f; // Multiplier for angular velocity
    [Tooltip("The rate at which the fish naturally slows its turn.")]
    [SerializeField] private float angularDamping = 5f;

    [Header("Inertia Settings")]
    [Tooltip("Taux de douceur/vitesse avec lequel la rotation cible est atteinte. Plus la valeur est petite, plus l'inertie est grande (réaction lente).")]
    [SerializeField] private float rotationInertiaRate = 10f;

    // --- Private Input Variable ---
    
    // Stores the current input value for rotation (-1 for left, 1 for right).
    private float rotationInputValue;

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
        float targetAngularVelocity = -rotationInputValue * rotationSpeedMultiplier;

        // 2. --- Apply Rotation with Interpolation (Inertie) ---
        float newAngularVelocity = Mathf.Lerp(
            rigidBody.angularVelocity, 
            targetAngularVelocity,      
            Time.fixedDeltaTime * rotationInertiaRate // Utilisation de la variable
        );

        rigidBody.angularVelocity = newAngularVelocity;
    }

    public void OnRotation(InputAction.CallbackContext context)
    {
        Debug.Log("OnRotation appelée.");
        // Reads the float value from the 1D Axis (A/D composite or Left Stick X-axis).
        rotationInputValue = context.ReadValue<float>();
        Debug.Log("rotationInputValue lue : " + rotationInputValue);
    }

}