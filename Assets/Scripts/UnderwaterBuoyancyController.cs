using UnityEngine;

public class UnderwaterBuoyancyController : MonoBehaviour
{
    [Header("floatation Setting")]
    public float baseBuoyancy = 5f;          // Basic buoyancy
    public float maxBuoyancy = 15f;          // Maximum buoyancy
    public float minBuoyancy = -5f;          // Minimum buoyancy
    public float buoyancyAdjustSpeed = 3f;   // QE adjustment speed

    [Header("Horizontal Movement Settings")]
    public float maxLateralForce = 10f;      // Max lateral force for horizontal movement]
    public float lateralForceMultiplier = 1.4f; // Multiplier for lateral force based on depth
    [Header("Depth Settings")]
    public float surfaceDepth = 0f;          // Water surface depth
    public float maxEffectiveDepth = 20f;    // Max depth for buoyancy effect
    public float depthExponent = 2f;         // Depth effect exponent
    
    [Header("Scale Settings")]
    public float minScale = 5f;
    public float maxScale = 8f;
    public float scaleSmoothing = 5f;
    
    [Header("Runtime Variables")]
    [SerializeField] private float currentBuoyancy = 0f;
    [SerializeField] private float currentDepth = 0f;
    [SerializeField] private float depthMultiplier = 1f;
    [SerializeField] private float lateralForce = 0f;
    [SerializeField] private float currentVerticalSpeed = 0f;
    
    private Rigidbody2D FishRb;
    private float targetBuoyancy = 0f;
    private float currentStamina { get;  set; }
    private StaminaSystem readCurrentStamina;
    void Start()
    {
        FishRb = GetComponent<Rigidbody2D>();
        currentBuoyancy = baseBuoyancy; //current private set=0?
        targetBuoyancy = baseBuoyancy;
        readCurrentStamina = FindAnyObjectByType<StaminaSystem>();
    }

    void Update()
    {
        currentStamina = readCurrentStamina.CurrentStamina;
        CalculateDepthEffect();
        HandleBuoyancyInput();
        OverrideBuoyancy();
        ApplyBuoyancyForce();
        UpdateScaleFromBuoyancy();
    }

    void FixedUpdate()
    {
        ApplyBuoyancyForce();
        ApplyLateralMovement();
        // UpdateDebugInfo();
    }

    /// <summary>
    /// QE for buoyancy adjustment
    /// </summary>
    private void HandleBuoyancyInput()
    {
        float adjustment = 0f;
        
        if (Input.GetKey(KeyCode.J))
        {
            adjustment = -buoyancyAdjustSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.L))
        {
            adjustment = buoyancyAdjustSpeed * Time.deltaTime;
        }
        
        if (adjustment != 0f)
        {
            // 应用深度影响后的调整量
            float adjustedChange = adjustment * depthMultiplier;
            targetBuoyancy = Mathf.Clamp(targetBuoyancy + adjustedChange, minBuoyancy, maxBuoyancy);
        }
        
        // smooth transition to target buoyancy
        currentBuoyancy = Mathf.Lerp(currentBuoyancy, targetBuoyancy, Time.deltaTime * 3f);
    }
    
    /// <summary>
    /// Calculate depth effect on buoyancy
    /// </summary>
    private void CalculateDepthEffect()
    {
        // calculate current depth（water surface y=0）
        currentDepth = Mathf.Max(0f, -transform.position.y - surfaceDepth);
        
        // Calculate depth multiplier: the deeper, the smaller the multiplier
        float normalizedDepth = Mathf.Clamp01(currentDepth / maxEffectiveDepth);
        depthMultiplier = Mathf.Pow(1f - normalizedDepth, depthExponent);
        
        // Clamp depth multiplier to avoid zero or negative values
        depthMultiplier = Mathf.Clamp(depthMultiplier, 0.1f, 1f);
    }

    private void UpdateScaleFromBuoyancy()
    {
        // On normalise currentBuoyancy entre -3 et +3 vers 0..1
        float normalized = Mathf.InverseLerp(-3f, 3f, currentBuoyancy);

        // On map 0..1 vers minScale..maxScale
        float targetScale = Mathf.Lerp(minScale, maxScale, normalized);

        // Application douce du scale
        Vector3 newScale = Vector3.one * targetScale;
        transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * scaleSmoothing);
    }

    /// <summary>
    /// Apply buoyancy force to the Rigidbody2D
    /// </summary>
    private void ApplyBuoyancyForce()
    {
        if (FishRb != null)
        {
            // Apply buoyancy（upward force）
            Vector2 buoyancyForce = Vector2.up * currentBuoyancy;
            FishRb.AddForce(buoyancyForce, ForceMode2D.Force);
        }
    }
    
    private float verticalForce(float currentBuoyancy)
    {
        if (currentBuoyancy >= 0)
        {
            return currentBuoyancy;
        }
        else
        {
            return -currentBuoyancy; //Maybe reduce the downforce effect
        }
    }
    private void ApplyLateralMovement()
    {
        if (FishRb == null) return;
        //Get current vertical speed
        // currentVerticalSpeed = FishRb.linearVelocity.y;
        //Get Z axis input(change to -180 to 180)
        float currentRotation = NormalizeAngle(transform.eulerAngles.z);
        float clampedRotation = Mathf.Clamp(currentRotation, -45f, 45f);
        float rotationRatio = -clampedRotation / 45f; // -1 to 1 Left to Right
        //Calculate lateral force based on Verticalforce
        float verticalForceApplied = verticalForce(currentBuoyancy);
        lateralForce = rotationRatio * lateralForceMultiplier * verticalForceApplied;
        //Apply lateral force
        Vector2 lateralForceVector = Vector2.right * lateralForce;
        FishRb.AddForce(lateralForceVector, ForceMode2D.Force);

    }
    /// <summary>
    /// NormalizeAngle to -180 to 180 
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }

    //buoyancy override by stamina systems

    public void OverrideBuoyancy()
    {
        if (currentStamina == 0)
        {
            targetBuoyancy = 0f;
        }
    }
    /// <summary>
    /// Get current buoyancy（For UI disaplay）
    /// </summary>
    public float GetCurrentBuoyancy()
    {
        return currentBuoyancy;
    }
    
    /// <summary>
    /// Get current depth（For UI disaplay）
    /// </summary>
    public float GetDepthMultiplier()
    {
        return depthMultiplier;
    }
    
    /// <summary>
    /// Override current buoyancy（For events）
    /// </summary>
    public void SetBuoyancy(float buoyancy)
    {
        targetBuoyancy = Mathf.Clamp(buoyancy, minBuoyancy, maxBuoyancy);
    }
    
    /// <summary>
    /// Reset buoyancy to base value
    /// </summary>
    public void ResetBuoyancy()
    {
        targetBuoyancy = baseBuoyancy;
    }
}