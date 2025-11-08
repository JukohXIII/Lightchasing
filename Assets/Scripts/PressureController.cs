using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PressureController : MonoBehaviour
{
    [Header("Pressure Settings")]
    public float maxPressure = 100f;
    public float pressureIncreaseRate = 0f;  // rate per second when ascending too fast
    public float pressureDecreaseRate = 10f;  // Base decrease rate
    public float safeAscendSpeedThreshold = 5f;  // max safe upward speed (units per second)
    public float maxDepth = 500f;
    private float pressureExponent = 2f;


    [Header("References")]
    public Rigidbody2D fishRigidbody;

    [Header("UI Elements")]
    public UnityEngine.UI.Image pressureBarFill;              // Image for Pressure Bar fill

    [Header("Runtime Variables")]
    public float currentPressure = 0f;
    public float currentDepth = 0f; // Positive number for depth
    
    
    public float PressurePercent => currentPressure / maxPressure;

    public UnityEvent OnPressureMaxReached; // Assign death event in inspector

    void Start()
    {
        if (fishRigidbody == null)
            fishRigidbody = GetComponent<Rigidbody2D>();
        
        UpdatePressureUI();
    }

    void Update()
    {
        
        // Update depth: assuming surface at y=0, depth is positive downward
        //currentDepth = Mathf.Clamp(transform.position.y, 0f, maxDepth);
        //Depth won't matter for pressure change
        pressureExponent = Mathf.Log(pressureDecreaseRate) / Mathf.Log(safeAscendSpeedThreshold);
        float verticalSpeed = fishRigidbody.linearVelocityY;

        if (verticalSpeed > safeAscendSpeedThreshold)
        {
            // Ascending too fast, increase pressure
            pressureIncreaseRate = Mathf.Pow(verticalSpeed, pressureExponent);
            currentPressure += pressureIncreaseRate * Time.deltaTime;
            Debug.Log($"{pressureIncreaseRate}");
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
        UpdatePressureUI();
    }
    
    //addinig pressure for dash
    public void AddPressure(float amount)
    {
        currentPressure += amount;
        UpdatePressureUI();
    }

    // Update pressure UI
    private void UpdatePressureUI()
    {
        if (pressureBarFill != null)
        {
            pressureBarFill.fillAmount = PressurePercent;
        }
    }
}
