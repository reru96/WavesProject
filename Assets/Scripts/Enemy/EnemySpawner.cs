using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("ScriptableObject Prefabs")]
    public CreatureSO enemySO;
    public CreatureSO obstacleSO;
    public CreatureSO enemySpecialSO;

    [Header("Spawn Settings")]
    public float spawnRate = 10f;
    public float spawnSpecialRate = 5f;
    public float yRange = 4f;
    public float xRange = 6f;
    public float spawnOffset = 10f;
    [Range(0f, 1f)] public float obstacleChance = 0.3f;

    private bool _playerReady = false;
    private Transform _player;

    private Coroutine _normalSpawnCoroutine;
    private Coroutine _specialSpawnCoroutine;

    private void Start()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady += OnPlayerReady;

        if (RespawnManager.Instance.Player != null)
            OnPlayerReady();
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady -= OnPlayerReady;
    }

    private void OnPlayerReady()
    {
        _player = RespawnManager.Instance.Player.transform;
        _playerReady = _player != null;

        if (_playerReady)
        {
            _normalSpawnCoroutine ??= StartCoroutine(SpawnRoutine(spawnRate, SpawnEntity));
            _specialSpawnCoroutine ??= StartCoroutine(SpawnRoutine(spawnSpecialRate, SpawnSpecialEnemy));
        }
    }

    private IEnumerator SpawnRoutine(float rate, Action spawnAction)
    {
        while (_playerReady)
        {
            spawnAction?.Invoke();
            yield return new WaitForSeconds(rate);
        }
    }

    private (Vector3 spawnPos, Vector2 moveDir) RandomDirection()
    {
        bool spawnObstacle = UnityEngine.Random.value < obstacleChance;
        CreatureSO soData = spawnObstacle ? obstacleSO : enemySO;

        Vector3 spawnPos = Vector3.zero;
        Vector2 moveDir = Vector2.zero;

        int dir = UnityEngine.Random.Range(0, 4);

        switch (dir)
        {
            case 0:
                spawnPos = new Vector3(_player.position.x + UnityEngine.Random.Range(-xRange, xRange),
                                       _player.position.y + yRange + spawnOffset, 0f);
                break;
            case 1:
                spawnPos = new Vector3(_player.position.x + UnityEngine.Random.Range(-xRange, xRange),
                                       _player.position.y - yRange - spawnOffset, 0f);
                break;
            case 2:
                spawnPos = new Vector3(_player.position.x + xRange + spawnOffset,
                                       _player.position.y + UnityEngine.Random.Range(-yRange, yRange), 0f);
                break;
            case 3:
                spawnPos = new Vector3(_player.position.x - xRange - spawnOffset,
                                       _player.position.y + UnityEngine.Random.Range(-yRange, yRange), 0f);
                break;
        }

        return (spawnPos, moveDir);
    }

    private void SpawnEntity()
    {
        bool spawnObstacle = UnityEngine.Random.value < obstacleChance;
        CreatureSO soData = spawnObstacle ? obstacleSO : enemySO;

        if (soData == null || soData.prefab == null) return;

        var (spawnPos, moveDir) = RandomDirection();

        ObjectPooler.Instance.Spawn(soData, spawnPos, Quaternion.identity);
    }

    public void SpawnSpecialEnemy()
    {
        if (enemySpecialSO == null || enemySpecialSO.prefab == null) return;

        var (spawnPos, moveDir) = RandomDirection();

        GameObject go = ObjectPooler.Instance.Spawn(enemySpecialSO, spawnPos, Quaternion.identity);

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
            enemy.SetDirection(moveDir);
    }

}

