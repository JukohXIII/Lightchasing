using UnityEngine;
using UnityEngine.UI; 

public class DepthMeter : MonoBehaviour
{
    public Transform player; 
    public Text distanceText;
    private float startY;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player transform non assigné !");
            enabled = false;
            return;
        }
        startY = player.position.y;
    }

    void Update()
    {
        float distance = player.position.y - startY;

        if (distance < 0) distance = 0; 

        int meters = Mathf.FloorToInt(distance);

        distanceText.text = meters + "m";
    }
}
