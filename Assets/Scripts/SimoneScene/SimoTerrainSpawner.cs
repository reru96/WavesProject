using System.Collections;
using UnityEngine;

public class SimoTerrainSpawner : MonoBehaviour
{
    [Header("Prefab e intervallo")]
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Area di spawn")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -6f;
    [SerializeField] private float maxY = -3f;

    [Header("Distanza minima")]
    [SerializeField] private float minDistance = 2f;

    private Vector3 lastSpawnPosition;

    private void Start()
    {
        lastSpawnPosition = new Vector3(-5, -5, 0);
        SpawnTerrain(lastSpawnPosition);
        StartCoroutine(SpawnTerrainLoop());
    }

    public void SpawnTerrain(Vector3 position)
    {
        Instantiate(terrainPrefab, position, Quaternion.identity);
        lastSpawnPosition = position;
    }

    public IEnumerator SpawnTerrainLoop()
    {
        while (true)
        {
            Vector3 spawnPosition = GenerateValidPosition();
            SpawnTerrain(spawnPosition);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GenerateValidPosition()
    {
        Vector3 candidate;
        int attempts = 0;
        do
        {
            candidate = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                0f
            );
            attempts++;
            if (attempts > 100) break; // evita loop infinito
        }
        while (Vector3.Distance(candidate, lastSpawnPosition) < minDistance);

        return candidate;
    }
}