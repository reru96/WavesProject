using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("Creature Data")]
    public CreatureSO[] enemySO;
    public CreatureSO[] enemySpecialSO;

    [Header("Spawn Rate")]
    public float spawnRate = 2f;
    public float spawnSpecialRate = 5f;

    [Header("Position Settings")]
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
        {
            RespawnManager.Instance.OnPlayerSpawned += OnPlayerSpawned;
            if (RespawnManager.Instance.Player != null)
            {
                OnPlayerSpawned(RespawnManager.Instance.Player);
            }
        }
        
        RespawnManager.OnGameOver += StopCoroutines;
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerSpawned -= OnPlayerSpawned;

        RespawnManager.OnGameOver -= StopCoroutines;

        StopCoroutines();
    }

    private void OnPlayerSpawned(GameObject newPlayer)
    {
        if (newPlayer == null) return;

        player = newPlayer.transform;

        StopCoroutines();

        if (player.gameObject.activeInHierarchy)
        {
            normalSpawn = StartCoroutine(SpawnRoutine(spawnRate, SpawnEntity));
            specialSpawn = StartCoroutine(SpawnRoutine(spawnSpecialRate, SpawnSpecialEnemy));
        }
    }


    private IEnumerator SpawnRoutine(float rate, Action spawnAction)
    {
        while (player != null && player.gameObject.activeInHierarchy)
        {
            if (!isActiveAndEnabled) yield break;

            spawnAction?.Invoke();
            yield return new WaitForSeconds(rate);
        }
        StopCoroutines();
    }


    private void SpawnEntity()
    {
        if (player == null || !player.gameObject.activeInHierarchy || enemySO.Length == 0) return;

        CreatureSO so = enemySO[UnityEngine.Random.Range(0, enemySO.Length)];

        float yPos = GetYPositionForColor(so.colorID, player.position.y);

        Vector3 pos = new Vector3(player.position.x + spawnOffsetX, yPos, 0);

        SpawnAndInitializeEnemy(so, pos);
    }

    private void SpawnSpecialEnemy()
    {
        if (player == null || !player.gameObject.activeInHierarchy || enemySpecialSO.Length == 0) return;

        CreatureSO so = enemySpecialSO[_specialEnemyIndex];

        float yPos = player.position.y + UnityEngine.Random.Range(-specialYRange, specialYRange);

        Vector3 pos = new Vector3(player.position.x + spawnOffsetX, yPos, 0);

        SpawnAndInitializeEnemy(so, pos);

        _specialEnemyIndex = (_specialEnemyIndex + 1) % enemySpecialSO.Length;
    }

    private void SpawnAndInitializeEnemy(CreatureSO data, Vector3 pos)
    {
        GameObject go = ObjectPooler.Instance.Spawn(data, pos, Quaternion.identity);

        if (go == null)
        {
            return;
        }

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

                return centerY + (Random.value < 0.5f ? -1.0f : 1.0f);

            case ColorType.Blue:
 
                return centerY + (Random.value < 0.5f ? -(yRangeLanes + 1f) : (yRangeLanes + 1f));

            case ColorType.Yellow:
                return centerY + 2f; 

            case ColorType.White:
                return centerY + Random.Range(-yRangeLanes, yRangeLanes);

            default:
                return centerY;
        }
    }

}
