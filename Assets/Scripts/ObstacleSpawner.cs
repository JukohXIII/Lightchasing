using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float spawnIntervalVariance = 0.5f;

    public Vector2 spawnAreaCenter = Vector2.zero;
    public Vector2 spawnAreaSize = new(10f, 1f);

    private float nextSpawnTime;

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

        Vector2 spawnPos = spawnAreaCenter +
                           new Vector2(
                               Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                               Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
                           );

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}