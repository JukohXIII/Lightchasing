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
    [SerializeField] private float accelerationRate = 200f; // valeur d'accélération pour MoveTowards (deg/s² effectif)
    [SerializeField] private float decelerationRate = 200f;
    [SerializeField] private float maxRotationAngle = 45f;

    private float rotationInputValue;
    private float currentAngularVelocity; // champ, pas d'ombrage

    void Start()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.angularDamping = angularDamping;
        rigidBody.gravityScale = 0f;
    }

    // Normalize angle into [-180, 180]
    private float NormalizeAngle(float a)
    {
        if (a > 180f) a -= 360f;
        return a;
    }

    void FixedUpdate()
    {
        // lire la vitesse actuelle (en deg/s)
        currentAngularVelocity = rigidBody.angularVelocity;

        // angle actuel normalisé
        float currentAngleZ = NormalizeAngle(transform.localEulerAngles.z);

        // vitesse désirée (attention au signe : ajuste si le sens est inversé pour toi)
        float desiredAngularVelocity = -rotationInputValue * rotationSpeedMultiplier;

        // --- Bloquer seulement si la vitesse désirée pousserait plus loin que la limite ---
        bool atLeftLimit  = currentAngleZ <= -maxRotationAngle;
        bool atRightLimit = currentAngleZ >=  maxRotationAngle;

        if (atLeftLimit && desiredAngularVelocity < 0f)
        {
            // on pousse davantage vers la gauche -> on empêche
            desiredAngularVelocity = 0f;
        }
        else if (atRightLimit && desiredAngularVelocity > 0f)
        {
            // on pousse davantage vers la droite -> on empêche
            desiredAngularVelocity = 0f;
        }

        // Si pas d'input, la cible est 0 (décélération)
        if (Mathf.Approximately(rotationInputValue, 0f))
            desiredAngularVelocity = 0f;

        // Smooth mais ferme : approche de la vitesse désirée (accélération / décélération)
        float accel = (Mathf.Abs(desiredAngularVelocity) > Mathf.Abs(currentAngularVelocity)) ? accelerationRate : decelerationRate;
        float newAngularVelocity = Mathf.MoveTowards(currentAngularVelocity, desiredAngularVelocity, accel * Time.fixedDeltaTime);

        rigidBody.angularVelocity = newAngularVelocity;

        // --- correction d'angle si on dépasse légèrement (sécurité) ---
        float clampedAngleZ = Mathf.Clamp(currentAngleZ, -maxRotationAngle, maxRotationAngle);
        if (!Mathf.Approximately(currentAngleZ, clampedAngleZ))
        {
            // appliquer correction visuelle
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, clampedAngleZ);
            // arrêter la rotation si on était en train de pousser au-delà
            if ((currentAngleZ < -maxRotationAngle && rotationInputValue < 0f) ||
                (currentAngleZ >  maxRotationAngle && rotationInputValue > 0f))
            {
                rigidBody.angularVelocity = 0f;
            }
        }
    }

    public void OnRotation(InputAction.CallbackContext context)
    {
        rotationInputValue = context.ReadValue<float>();
    }
}
