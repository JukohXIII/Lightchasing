using UnityEngine;

public class DashMechanic : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashDistance = 1f;           // Dash distance
    public float dashDuration = 0.1f;         // Dash duration
    public float dashCooldown = 1f;           // Cooldown time
    public int maxDashCharges = 2;            // Maximum dash charges
    public float chargeRecoveryTime = 2f;     // Charge recovery time 

    [Header("Visual Effects")]
    public GameObject dashTrailEffect;        // Dash trail effect
    public AnimationCurve dashSpeedCurve;     // Dash speed curve

    [Header("Inertia Settings")]
    public float inertiaDistance = 1f;
    public float inertiaDuration = 0.1f;
    public AnimationCurve inertiaCurve;

    [Header("Runtime Variables")]
    [SerializeField] private int currentDashCharges;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool inInertia = false;
    [SerializeField] private float cooldownTimer = 0f;
    [SerializeField] private float recoveryTimer = 0f;

    // Public porperties for read
    public bool IsDashing => isDashing;
    public int CurrentDashCharges => currentDashCharges;
    public bool InInertia => inInertia;
    public bool IsMoving => isDashing || inInertia;
    public bool CanDash => canDash && currentDashCharges > 0 && !isDashing && !inInertia;

    private StaminaSystem staminaSystem;
    private float dashStaminaCost = 50f;      // stamina cost per dash
    private Vector2 lastDashDirection;
    // Dash events
    public System.Action OnDashStart;
    public System.Action OnDashEnd;
    public System.Action OnInertiaStart;
    public System.Action OnInertiaEnd;
    public System.Action OnDashChargesChanged;

    void Start()
    {
        staminaSystem = FindAnyObjectByType<StaminaSystem>();
        currentDashCharges = maxDashCharges;

        // Initialize speed curve if not set
        if (dashSpeedCurve == null || dashSpeedCurve.length == 0)
        {
            dashSpeedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }
        
        // Initialize inertia curve if not set
        if (inertiaCurve == null || inertiaCurve.length == 0)
        {
            inertiaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }
    }

    void Update()
    {
        HandleDashInput();
        UpdateCooldowns();
    }

    /// <summary>
    /// Dash input handling
    /// </summary>
    private void HandleDashInput()
    {
        // Detect dash input
        if (Input.GetKeyDown(KeyCode.Space) && CanDash)
        {
            AttemptDash();
        }
    }

    /// <summary>
    /// Attempt to start a dash
    /// </summary>
    public void AttemptDash()
    {
        // Check stamina
        if (staminaSystem != null && !staminaSystem.HasEnoughStamina(dashStaminaCost))
        {
            Debug.Log("not enough stamina to dash");
            return;
        }

        StartDash();
    }

    /// <summary>
    /// Start Dash
    /// </summary>
    private void StartDash()
    {
        if (isDashing) return;

        // Consume stamina
        if (staminaSystem != null)
        {
            staminaSystem.ConsumeStamina(dashStaminaCost);
        }

        // Consume dash charge
        currentDashCharges--;
        //Invoke dash charges changed event(Dash UI update)
        // OnDashChargesChanged?.Invoke();

        // 开始冲刺协程
        StartCoroutine(PerformDash());

        // 触发事件
        // OnDashStart?.Invoke();

        Debug.Log($"Start Dash, remaining dash: {currentDashCharges}");
    }

    /// <summary>
    /// Execute the dash over time
    /// </summary>
    private System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        // Save start position
        Vector3 startPosition = transform.position;
        lastDashDirection = GetDashDirection();
        // Calculate dash direction（Based on Z rotation）
        Vector2 dashDirection = GetDashDirection();
        Vector3 targetPosition = startPosition + (Vector3)dashDirection * dashDistance;

        // Activate visual effects
        if (dashTrailEffect != null)
        {
            dashTrailEffect.SetActive(true);
        }

        float elapsedTime = 0f;

        // Dash loop
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dashDuration;
            float curveValue = dashSpeedCurve.Evaluate(t);

            // Interpolate position
            transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null;
        }

        // Make sure we reach the target position
        transform.position = targetPosition;

        // End dash
        EndDash();
    }

    /// <summary>
    /// calculate dash direction based on current rotation
    /// </summary>
    private Vector2 GetDashDirection()
    {
        // get rotation angle（standarlize to -180 to 180）
        float currentRotation = -NormalizeAngle(transform.eulerAngles.z);
        
        // Turn angle to direction vector
        float angleInRadians = currentRotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
        
        return direction.normalized;
    }

    /// <summary>
    /// End Dash
    /// </summary>
    private void EndDash()
    {
        isDashing = false;

        // Ban visual effects
        if (dashTrailEffect != null)
        {
            dashTrailEffect.SetActive(false);
        }

        // Start cooldown
        cooldownTimer = dashCooldown;

        // Invoke end event
        // OnDashEnd?.Invoke();
        StartCoroutine(PerformInertia());

        Debug.Log("End Dash");
    }

    private System.Collections.IEnumerator PerformInertia()
    {
        inInertia = true;
        // Invoke inertia start event
        // OnInertiaStart?.Invoke();

        Vector3 startPosition = transform.position;
        Vector2 inertiaDirection = GetDashDirection();
        Vector3 targetPosition = startPosition + (Vector3)inertiaDirection * inertiaDistance;

        float elapsedTime = 0f;

        // Inertia loop
        while (elapsedTime < inertiaDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / inertiaDuration;
            float curveValue = inertiaCurve.Evaluate(t);

            // Interpolate position
            transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null;
        }

        // Ensure we reach the target position
        transform.position = targetPosition;

        EndInertia();
        // Invoke inertia end event
        // OnInertiaEnd?.Invoke();

        Debug.Log("End Inertia");
    }

    private void EndInertia()
    {
        inInertia = false;
    }

    /// <summary>
    /// Update cooldowns and recovery
    /// </summary>
    private void UpdateCooldowns()
    {
        // Cooldown timer
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canDash = true;
            }
        }

        // Dash charge recovery timer
        if (currentDashCharges < maxDashCharges)
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= chargeRecoveryTime)
            {
                currentDashCharges++;
                recoveryTimer = 0f;
                //OnDashChargesChanged?.Invoke();
                Debug.Log($"Dash charge recovered current dash amount: {currentDashCharges}");
            }
        }
        else
        {
            recoveryTimer = 0f;
        }
    }

    /// <summary>
    /// Normalize angle
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }

    /// <summary>
    /// 强制重置冲刺状态（用于复活等场景）
    /// </summary>
    // public void ResetDash()
    // {
    //     StopAllCoroutines();
    //     isDashing = false;
    //     canDash = true;
    //     currentDashCharges = maxDashCharges;
    //     cooldownTimer = 0f;
    //     recoveryTimer = 0f;

    //     if (dashTrailEffect != null)
    //     {
    //         dashTrailEffect.SetActive(false);
    //     }

    //     OnDashChargesChanged?.Invoke();
    // }

    /// <summary>
    /// 立即恢复一次冲刺次数（用于道具）
    /// </summary>
    // public void RestoreDashCharge()
    // {
    //     if (currentDashCharges < maxDashCharges)
    //     {
    //         currentDashCharges++;
    //         OnDashChargesChanged?.Invoke();
    //     }
    // }

    /// <summary>
    /// 外部调用冲刺（用于AI或其他系统）
    /// </summary>
    // public void ExecuteDash(Vector2 direction)
    // {
    //     if (CanDash)
    //     {
    //         // 临时设置方向并冲刺
    //         float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
    //         transform.rotation = Quaternion.Euler(0f, 0f, -targetAngle);
    //         StartDash();
    //     }
    // }
}
