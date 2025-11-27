using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public CreatureSO[] enemySO;
    public CreatureSO[] enemySpecialSO;
    public float spawnRate = 2f;
    public float spawnSpecialRate = 5f;
    public float spawnOffsetX = 12f;
    public float yRangeLanes = 4f;
    public float specialYRange = 4f;

    private Transform player;
    private int _specialEnemyIndex = 0;
    private Coroutine normalSpawn;
    private Coroutine specialSpawn;

    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerSpawned += OnPlayerSpawned;
        RespawnManager.OnGameOver += StopCoroutines;
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerSpawned -= OnPlayerSpawned;
        RespawnManager.OnGameOver -= StopCoroutines;
    }

    private void OnPlayerSpawned(GameObject newPlayer)
    {
        player = newPlayer.transform;
        StopCoroutines();
        normalSpawn = StartCoroutine(SpawnRoutine(spawnRate, SpawnEntity));
        specialSpawn = StartCoroutine(SpawnRoutine(spawnSpecialRate, SpawnSpecialEnemy));
    }

    private IEnumerator SpawnRoutine(float rate, Action spawnAction)
    {
        while (player != null)
        {
            spawnAction?.Invoke();
            yield return new WaitForSeconds(rate);
        }
    }

    private void SpawnEntity()
    {
        if (player == null || enemySO.Length == 0) return;
        CreatureSO so = enemySO[UnityEngine.Random.Range(0, enemySO.Length)];
        float y = player.position.y + UnityEngine.Random.Range(-yRangeLanes, yRangeLanes);
        Vector3 pos = new Vector3(player.position.x + spawnOffsetX, y, 0);
        SpawnAndInitializeEnemy(so, pos);
    }

    private void SpawnSpecialEnemy()
    {
        if (player == null || enemySpecialSO.Length == 0) return;
        CreatureSO so = enemySpecialSO[_specialEnemyIndex];
        float y = player.position.y + UnityEngine.Random.Range(-specialYRange, specialYRange);
        Vector3 pos = new Vector3(player.position.x + spawnOffsetX, y, 0);
        SpawnAndInitializeEnemy(so, pos);
        _specialEnemyIndex = (_specialEnemyIndex + 1) % enemySpecialSO.Length;
    }

    private void SpawnAndInitializeEnemy(CreatureSO data, Vector3 pos)
    {
        GameObject go = ObjectPooler.Instance.Spawn(data, pos, Quaternion.identity);
        Enemy e = go.GetComponent<Enemy>();
        if (e != null)
        {
            e.Initialize(data.EnemySprite, data.colorID);
            e.SetDirection(Vector2.left);
        }
    }

    private void StopCoroutines()
    {
        if (normalSpawn != null) { StopCoroutine(normalSpawn); normalSpawn = null; }
        if (specialSpawn != null) { StopCoroutine(specialSpawn); specialSpawn = null; }
    }
}
