using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    public Transform playerTransform;

    public float baseSpawnInterval = 2f;
    public float spawnIntervalVariance = 0.5f;

    private float nextSpawnTime;

    public float difficultyScale = 0.02f; 
    // Plus cette valeur est grande, plus la difficulté augmente vite

    private Camera mainCamera;
    public float spawnOffset = 2f;

    public float safeRadius = 1.5f;
    public float spawnInterval = 2f;

    private Vector2 spawnAreaMin;
    private Vector2 spawnAreaMax;
    
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        float difficulty = CalculateDifficulty();

        float currentInterval = Mathf.Max(0.2f, baseSpawnInterval / difficulty);

        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();

            nextSpawnTime = Time.time + Random.Range(
                currentInterval - spawnIntervalVariance,
                currentInterval + spawnIntervalVariance
            );
        }
    }

    float CalculateDifficulty()
    {
        float height = playerTransform.position.y;

        return 1f + (height * difficultyScale);
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0 || playerTransform == null) return;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        int attempts = 0;
        const int maxAttempts = 10;  // Pour éviter boucle infinie

        while (attempts < maxAttempts)
        {
            int side = Random.Range(0, 4); // 0=left,1=right,2=top,3=bottom
            Vector2 spawnPos = Vector2.zero;

            switch (side)
            {
                case 0: // Left
                    spawnPos = new Vector2(bottomLeft.x - spawnOffset,
                                        Random.Range(bottomLeft.y, topRight.y));
                    break;
                case 1: // Right
                    spawnPos = new Vector2(topRight.x + spawnOffset,
                                        Random.Range(bottomLeft.y, topRight.y));
                    break;
                case 2: // Up
                    spawnPos = new Vector2(Random.Range(bottomLeft.x, topRight.x),
                                        topRight.y + spawnOffset);
                    break;
                case 3: // Down
                    spawnPos = new Vector2(Random.Range(bottomLeft.x, topRight.x),
                                        bottomLeft.y - spawnOffset);
                    break;
            }

            if (Vector2.Distance(spawnPos, playerTransform.position) > safeRadius)
            {
                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                Instantiate(prefab, spawnPos, Quaternion.identity);
                return;  // Spawn réussi, on sort
            }

            attempts++;
        }
    }
}
