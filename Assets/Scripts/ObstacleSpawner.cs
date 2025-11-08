using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    public float spawnInterval = 2f;
    public float spawnIntervalVariance = 0.5f;

    private float nextSpawnTime;

    private Camera mainCamera;
    private Vector2 spawnAreaMin;
    private Vector2 spawnAreaMax;

    public float spawnOffset = 2f;  // distance hors écran pour spawn

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            nextSpawnTime = Time.time + Random.Range(
                spawnInterval - spawnIntervalVariance,
                spawnInterval + spawnIntervalVariance
            );
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0) return;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

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

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
