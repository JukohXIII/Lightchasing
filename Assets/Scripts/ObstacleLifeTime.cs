using UnityEngine;

public class ObstacleLifeTime : MonoBehaviour
{
    public float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}