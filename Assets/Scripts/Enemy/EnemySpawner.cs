using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("ScriptableObject Prefabs")]
    public CreatureSO[] enemySO;       
    public CreatureSO[] enemySpecialSO;

    [Header("Spawn Settings")]
    public float spawnRate = 2f;       
    public float spawnSpecialRate = 5f;

    [Header("Position Settings")]
    public float spawnOffsetX = 12f;   
    public float yRangeLanes = 4f;     
    public float specialYRange = 4f;   

    private int _specialEnemyIndex = 0;
    private bool _playerReady = false;
    private Transform _player;

    private Coroutine _normalSpawnCoroutine;
    private Coroutine _specialSpawnCoroutine;

    private void Start()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady += OnPlayerReady;

        if (RespawnManager.Instance != null && RespawnManager.Instance.Player != null)
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
            if (_normalSpawnCoroutine == null)
                _normalSpawnCoroutine = StartCoroutine(SpawnRoutine(spawnRate, SpawnEntity));

            if (_specialSpawnCoroutine == null)
                _specialSpawnCoroutine = StartCoroutine(SpawnRoutine(spawnSpecialRate, SpawnSpecialEnemy));
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

    private void SpawnEntity()
    {
        if (_player == null) return;
        if (enemySO == null || enemySO.Length == 0) return;

        CreatureSO soData = enemySO[Random.Range(0, enemySO.Length)];
        if (soData == null || soData.prefab == null) return;

        float yPos = GetYPositionForColor(soData.colorID, _player.position.y);
        float xPos = _player.position.x + spawnOffsetX;

        Vector3 spawnPos = new Vector3(xPos, yPos, 0f);
        SpawnAndInitializeEnemy(soData, spawnPos);
    }

    public void SpawnSpecialEnemy()
    {
        if (_player == null) return;
        if (enemySpecialSO == null || enemySpecialSO.Length == 0) return;

        CreatureSO specialData = enemySpecialSO[_specialEnemyIndex];

        float randomYOffset = Random.Range(-specialYRange, specialYRange);
        float yPos = _player.position.y + randomYOffset;
        float xPos = _player.position.x + spawnOffsetX;

        Vector3 spawnPos = new Vector3(xPos, yPos, 0f);

        SpawnAndInitializeEnemy(specialData, spawnPos);
        _specialEnemyIndex = (_specialEnemyIndex + 1) % enemySpecialSO.Length;
    }

    private void SpawnAndInitializeEnemy(CreatureSO data, Vector3 pos)
    {
        GameObject go = ObjectPooler.Instance.Spawn(data, pos, Quaternion.identity);

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(data.EnemySprite, data.colorID);

            enemy.SetDirection(Vector2.left);
        }
    }

    private float GetYPositionForColor(ColorType color, float centerY)
    {
        switch (color)
        {
            case ColorType.Red:
                return centerY; 

            case ColorType.Purple:
                return centerY + yRangeLanes; 

            case ColorType.Cyan:
                return centerY - yRangeLanes; 

            case ColorType.Green:
                return centerY + (Random.value < 0.5f ? -2.5f : 2.5f);

            case ColorType.Orange:
                return centerY + (Random.value < 0.5f ? -1.2f : 1.2f);

            case ColorType.Blue:
                return centerY + (Random.value < 0.5f ? -4.5f : 4.5f);

            case ColorType.Yellow:
                return centerY + 2f;

            case ColorType.White:
                return centerY + Random.Range(-yRangeLanes, yRangeLanes);

            default:
                return centerY;
        }
    }
}
