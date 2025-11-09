using UnityEngine;
using System.Collections;

public class DashMechanic : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashImpulseForce = 30f;      // 改为冲量力
    public float dashDuration = 0.1f;         // 冲刺持续时间
    public float dashCooldown = 1f;           // 冷却时间
    public int maxDashCharges = 2;            // 最大冲刺次数
    public float chargeRecoveryTime = 2f;     // 次数恢复时间

    [Header("Inertia Settings")]
    public float inertiaForce = 15f;          // 惯性力大小
    public float inertiaDuration = 0.3f;      // 惯性持续时间
    public AnimationCurve inertiaForceCurve;  // 惯性力曲线

    [Header("Visual Effects")]
    public GameObject dashTrailEffect;        // 冲刺拖尾效果

    [Header("Runtime Variables")]
    [SerializeField] private int currentDashCharges;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool inInertia = false;
    [SerializeField] private float cooldownTimer = 0f;
    [SerializeField] private float recoveryTimer = 0f;

    // Public properties for read - 保持完全一致
    public bool IsDashing => isDashing;
    public int CurrentDashCharges => currentDashCharges;
    public bool InInertia => inInertia;
    public bool IsMoving => isDashing || inInertia;
    public bool CanDash => canDash && currentDashCharges > 0 && !isDashing && !inInertia;

    private StaminaSystem staminaSystem;
    private PressureController pressureController;
    private Rigidbody2D rb; // 新增：物理刚体引用
    private float dashStaminaCost = 25f;
    private Vector2 lastDashDirection;

    // Dash events - 保持完全一致
    public System.Action OnDashStart;
    public System.Action OnDashEnd;
    public System.Action OnInertiaStart;
    public System.Action OnInertiaEnd;
    public System.Action OnDashChargesChanged;

    void Start()
    {
        pressureController = FindAnyObjectByType<PressureController>(); 
        staminaSystem = FindAnyObjectByType<StaminaSystem>();
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
        currentDashCharges = maxDashCharges;

        // 初始化惯性力曲线
        if (inertiaForceCurve == null || inertiaForceCurve.length == 0)
        {
            inertiaForceCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f); // 从强到弱
        }
    }

    void Update()
    {
        HandleDashInput();
        UpdateCooldowns();
    }

    /// <summary>
    /// Dash input handling - 保持不变
    /// </summary>
    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanDash)
        {
            AttemptDash();
        }
    }

    /// <summary>
    /// Attempt to start a dash - 保持不变
    /// </summary>
    public void AttemptDash()
    {
        if (staminaSystem != null && !staminaSystem.HasEnoughStamina(dashStaminaCost))
        {
            Debug.Log("not enough stamina to dash");
            return;
        }

        StartDash();
    }

    /// <summary>
    /// Start Dash - 事件触发部分保持不变
    /// </summary>
    private void StartDash()
    {
        if (isDashing) return;

        // Consume stamina - 保持不变
        if (staminaSystem != null)
        {
            staminaSystem.ConsumeStamina(dashStaminaCost);
        }

        // Consume dash charge - 保持不变
        currentDashCharges--;

        // 开始冲刺协程
        StartCoroutine(PerformDash());

        // 触发事件 - 完全保持不变
        pressureController.AddPressure(20f); // Add pressure on dash

        Debug.Log($"Start Dash, remaining dash: {currentDashCharges}");
    }

    /// <summary>
    /// 基于物理的冲刺执行
    /// </summary>
    private System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        // 保存冲刺方向
        lastDashDirection = GetDashDirection();

        // 激活视觉效果 - 保持不变
        if (dashTrailEffect != null)
        {
            dashTrailEffect.SetActive(true);
        }

        // 应用冲刺冲量力
        if (rb != null)
        {
            rb.AddForce(lastDashDirection * dashImpulseForce, ForceMode2D.Impulse);
        }

        float elapsedTime = 0f;

        // 冲刺持续时间循环
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 结束冲刺
        EndDash();
    }

    /// <summary>
    /// calculate dash direction based on current rotation - 保持不变
    /// </summary>
    private Vector2 GetDashDirection()
    {
        float currentRotation = -NormalizeAngle(transform.eulerAngles.z);
        float angleInRadians = currentRotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
        return direction.normalized;
    }

    /// <summary>
    /// End Dash - 事件触发部分保持不变
    /// </summary>
    private void EndDash()
    {
        isDashing = false;

        // 禁用视觉效果 - 保持不变
        if (dashTrailEffect != null)
        {
            dashTrailEffect.SetActive(false);
        }

        // 开始冷却 - 保持不变
        cooldownTimer = dashCooldown;

        // 开始惯性阶段
        StartCoroutine(PerformInertia());

        Debug.Log("End Dash");
    }

    /// <summary>
    /// 基于物理的惯性执行
    /// </summary>
    private System.Collections.IEnumerator PerformInertia()
    {
        inInertia = true;
        
        // 触发惯性开始事件 - 保持不变
        // OnInertiaStart?.Invoke();

        float elapsedTime = 0f;

        // 惯性力应用循环
        while (elapsedTime < inertiaDuration && rb != null)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / inertiaDuration;
            float forceMultiplier = inertiaForceCurve.Evaluate(t);

            // 应用逐渐减弱的惯性力
            rb.AddForce(lastDashDirection * inertiaForce * forceMultiplier * Time.deltaTime, ForceMode2D.Force);
            
            yield return null;
        }

        // 结束惯性
        EndInertia();
        
        // 触发惯性结束事件 - 保持不变
        // OnInertiaEnd?.Invoke();

        Debug.Log("End Inertia");
    }

    /// <summary>
    /// End Inertia - 保持不变
    /// </summary>
    private void EndInertia()
    {
        inInertia = false;
    }

    /// <summary>
    /// Update cooldowns and recovery - 完全保持不变
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
    /// Normalize angle - 保持不变
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }

    // 所有注释掉的方法都保持原样不变
}