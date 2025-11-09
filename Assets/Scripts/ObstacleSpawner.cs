using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacles")]
    public GameObject[] obstaclePrefabs;
    public Transform playerTransform;

    private Camera mainCamera;
    public float spawnOffset = 2f;
    public float safeRadius = 1.5f;

    [Header("Spawn Settings")]
    public float maxSpawnInterval = 3f;      
    public float minSpawnInterval = 0.7f;  
    public float spawnIntervalVariance = 0.3f;

    [Header("Difficulty")]
    public float speedIncreasePerHeight = 0.003f;

    private float nextSpawnTime;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        float height = playerTransform.position.y;

        float t = Mathf.InverseLerp(0, 300, height);
        t = Mathf.Pow(t, 1.8f); // courbe exponentielle douce pour progression plus naturelle

        float currentInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, t);

        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacleWithDifficulty(t);

            nextSpawnTime = Time.time + Random.Range(
                currentInterval - spawnIntervalVariance,
                currentInterval + spawnIntervalVariance
            );
        }
    }

    void SpawnObstacleWithDifficulty(float spawnDensity)
    {
        SpawnObstacle();
        float doubleSpawnChance = spawnDensity * spawnDensity * 0.5f;

        if (Random.value < doubleSpawnChance)
        {
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0 || playerTransform == null) return;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        int attempts = 0;
        const int maxAttempts = 10;

        while (attempts < maxAttempts)
        {
            int side = Random.Range(0, 4);
            Vector2 spawnPos = Vector2.zero;

            switch (side)
            {
                case 0:
                    spawnPos = new Vector2(bottomLeft.x - spawnOffset,
                                           Random.Range(bottomLeft.y, topRight.y));
                    break;
                case 1:
                    spawnPos = new Vector2(topRight.x + spawnOffset,
                                           Random.Range(bottomLeft.y, topRight.y));
                    break;
                case 2:
                    spawnPos = new Vector2(Random.Range(bottomLeft.x, topRight.x),
                                           topRight.y + spawnOffset);
                    break;
                case 3:
                    spawnPos = new Vector2(Random.Range(bottomLeft.x, topRight.x),
                                           bottomLeft.y - spawnOffset);
                    break;
            }

            if (Vector2.Distance(spawnPos, playerTransform.position) > safeRadius)
            {
                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

                Obstacle mover = obj.GetComponent<Obstacle>();
                if (mover != null)
                {
                    mover.speedMultiplier = 1f + playerTransform.position.y * speedIncreasePerHeight;
                }

                return;
            }

            attempts++;
        }
    }
}
