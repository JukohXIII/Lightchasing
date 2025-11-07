using UnityEngine;

public class UnderwaterBuoyancyController : MonoBehaviour
{
    [Header("floatation Setting")]
    public float baseBuoyancy = 5f;          // Basic buoyancy
    public float maxBuoyancy = 15f;          // Maximum buoyancy
    public float minBuoyancy = -5f;          // Minimum buoyancy
    public float buoyancyAdjustSpeed = 3f;   // QE adjustment speed
    
    [Header("Depth Settings")]
    public float surfaceDepth = 0f;          // Water surface depth
    public float maxEffectiveDepth = 20f;    // Max depth for buoyancy effect
    public float depthExponent = 2f;         // Depth effect exponent
    
    [Header("Runtime Variables")]
    [SerializeField] private float currentBuoyancy = 0f;
    [SerializeField] private float currentDepth = 0f;
    [SerializeField] private float depthMultiplier = 1f;
    
    private Rigidbody2D FishRb;
    private float targetBuoyancy = 0f;
    
    void Start()
    {
        FishRb = GetComponent<Rigidbody2D>();
        currentBuoyancy = baseBuoyancy; //current private set=0?
        targetBuoyancy = baseBuoyancy;
    }
    
    void Update()
    {
        HandleBuoyancyInput();
        CalculateDepthEffect();
        ApplyBuoyancyForce();
    }
    
    /// <summary>
    /// QE for buoyancy adjustment
    /// </summary>
    private void HandleBuoyancyInput()
    {
        float adjustment = 0f;
        
        if (Input.GetKey(KeyCode.Q))
        {
            adjustment = -buoyancyAdjustSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
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