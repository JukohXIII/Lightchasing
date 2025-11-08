using UnityEngine;
using System.Collections.Generic;

public class SimpleOceanCurrent : MonoBehaviour
{
    [Header("洋流设置")]
    public Vector2 currentForce = new Vector2(5f, 0f);
    public ForceMode2D forceMode = ForceMode2D.Force;

    // 存储当前在洋流中的物体
    private HashSet<Rigidbody2D> objectsInCurrent = new HashSet<Rigidbody2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            objectsInCurrent.Add(rb);
        }
    }

    private void FixedUpdate()
    {
        // 只在FixedUpdate中为在洋流中的物体施加力
        foreach (var rb in objectsInCurrent)
        {
            if (rb != null)
            {
                rb.AddForce(currentForce, forceMode);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            objectsInCurrent.Remove(rb);
            
            // 可选：立即施加一个反向的小冲量来抵消动量
            // rb.AddForce(-currentForce * 0.3f, ForceMode2D.Impulse);
        }
    }

    // 调试图形...
}