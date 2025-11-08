using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;           // Maximum stamina value
    public float minStamina = 0f;             // Minimum stamina value
    public float depletionRate = 15f;         // Stamina depletion base rate
    public float recoveryRate = 20f;          // Stamina recovery base rate
    public float buoyancyThreshold = 0.5f;    // buoyancy threshold for stamina recovery

    [Header("UI Elements")]
    public Image staminaBarFill;              // Image for Stamina Bar fill
    public RectTransform staminaBarTransform; // Stamina bar stretch

    [Header("Status monitor")]
    [SerializeField] private float currentStamina;
    [SerializeField] private bool isExhausted = false;
    [SerializeField] private float currentBuoyancy = 0f;

    // Public for read
    public float CurrentStamina => currentStamina;
    public bool IsExhausted => isExhausted;
    public float StaminaPercentage => currentStamina / maxStamina;

    // Event, for other systems to subscribe
    // public System.Action OnStaminaDepleted;
    // public System.Action OnStaminaRecovered;

    private UnderwaterBuoyancyController buoyancyController;

    void Start()
    {
        // Initialize stamina
        currentStamina = maxStamina;
        
        // Find the buoyancy controller in the scene
        buoyancyController = FindAnyObjectByType<UnderwaterBuoyancyController>();
        
        // Initialize UI
        UpdateStaminaUI();
    }

    void Update()
    {
        if (buoyancyController != null)
        {
            // Get Current Buoyancy
            currentBuoyancy = buoyancyController.GetCurrentBuoyancy();
            UpdateStamina(currentBuoyancy);
        }
        
        UpdateStaminaUI();
        CheckExhaustionStatus();
    }

    /// <summary>
    /// update stamina based on current buoyancy
    /// </summary>
    private void UpdateStamina(float buoyancy)
    {
        if (buoyancy > buoyancyThreshold)
        {
            // stamina depletion
            float depletionMultiplier = Mathf.Clamp(buoyancy / maxStamina, 1f, 3f);
            float depletionAmount = depletionRate * depletionMultiplier * Time.deltaTime;
            currentStamina = Mathf.Max(minStamina, currentStamina - depletionAmount);
        }
        else
        {
            // stamina recovery
            float recoveryMultiplier = Mathf.Clamp(1f - buoyancy, 1f, 2f);
            float recoveryAmount = recoveryRate * recoveryMultiplier * Time.deltaTime;
            currentStamina = Mathf.Min(maxStamina, currentStamina + recoveryAmount);
        }
    }

    /// <summary>
    /// check and update exhaustion status
    /// </summary>
    private void CheckExhaustionStatus()
    {
        bool wasExhausted = isExhausted;
        isExhausted = currentStamina <= minStamina;

        // 触发(浮力变为0,主体大小根据所受浮力变化)
        if (!wasExhausted && isExhausted)
        {
            // OnStaminaDepleted?.Invoke();
            Debug.Log("Stamina depleted, no more buoyancy");
        }
        else if (wasExhausted && !isExhausted)
        {
            // OnStaminaRecovered?.Invoke();
            Debug.Log("Stamina recovered");
        }
    }

    /// <summary>
    /// Update the stamina UI elements
    /// </summary>
    private void UpdateStaminaUI()
    {
        if (staminaBarFill != null)
        {
            // Update fill amount
            staminaBarFill.fillAmount = StaminaPercentage;
        }

        if (staminaBarTransform != null)
        {
            // Update scale
            Vector2 newScale = staminaBarTransform.localScale;
            newScale.x = StaminaPercentage;
            staminaBarTransform.localScale = newScale;
        }
    }

    /// <summary>
    /// Stamina override（For items and events）
    /// </summary>
    // public void SetStamina(float stamina)
    // {
    //     currentStamina = Mathf.Clamp(stamina, minStamina, maxStamina);
    //     UpdateStaminaUI();
    // }

    /// <summary>
    /// Adding stamina（for items）
    /// </summary>
    // public void AddStamina(float amount)
    // {
    //     currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
    //     UpdateStaminaUI();
    // }

    /// <summary>
    /// subtract stamina（for special occasions）
    /// </summary>
    // public void ConsumeStamina(float amount)
    // {
    //     currentStamina = Mathf.Max(minStamina, currentStamina - amount);
    //     UpdateStaminaUI();
    // }

    /// <summary>
    /// Restore all stamina（for events） 
    /// </summary>
    // public void RestoreAllStamina()
    // {
    //     currentStamina = maxStamina;
    //     UpdateStaminaUI();
    // }

    /// <summary>
    /// Stamina check（for actions） 
    /// </summary>
    // public bool HasEnoughStamina(float requiredStamina)
    // {
    //     return currentStamina >= requiredStamina && !isExhausted;
    // }
}
