using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public Object enemySO;
    public Object obstacleSO;

    [Header("Spawn Settings")]
    public float spawnRate = 2f;
    public float yRange = 4f;           // larghezza orizzontale reale
    public float spawnOffsetX = 10f;    // quanto sopra il player (lungo l'asse X)
    [Range(0f, 1f)] public float obstacleChance = 0.3f;

    private float _timer;
    private Transform _player;

    void Start()
    {
        _player = RespawnManager.Instance.GetPlayer()?.transform;
        _timer = 0f;
    }

    void Update()
    {
        if (_player == null) return;

        _timer += Time.deltaTime;
        if (_timer >= spawnRate)
        {
            _timer = 0f;
            SpawnEntity();
        }
    }

    void SpawnEntity()
    {
        bool spawnObstacle = Random.value < obstacleChance;
        Object prefab = spawnObstacle ? obstacleSO : enemySO;

        if (prefab == null) return;

        float y = Random.Range(-yRange, yRange);
        float x = _player.position.x + spawnOffsetX;

        ObjectPooler.Instance.Spawn(prefab, new Vector3(x, y, 0f), Quaternion.identity);
    }
}

