using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("ScriptableObject Prefabs")]
    public Object enemySO;
    public Object obstacleSO;

    [Header("Spawn Settings")]
    public float spawnRate = 2f;
    public float yRange = 4f;             // larghezza verticale dello spawn
    public float spawnOffsetX = 10f;      // distanza davanti al player
    [Range(0f, 1f)] public float obstacleChance = 0.3f;

    private float _timer;
    private Transform _player;
    private bool _playerReady = false;

    private void Start()
    {
        _timer = 0f;
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady += OnPlayerReady;

        if (RespawnManager.Instance.GetPlayer() != null)
            OnPlayerReady();
    }

    void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady -= OnPlayerReady;
    }
    private void OnPlayerReady()
    {
        _player = RespawnManager.Instance.GetPlayer()?.transform;
        _playerReady = _player != null;
    }

    void Update()
    {

        if (!_playerReady) return;

        _timer += Time.deltaTime;
        if (_timer >= spawnRate)
        {
            _timer = 0f;
            SpawnEntity();
        }
    }

    private void SpawnEntity()
    {
        // Decidi se spawnare un ostacolo o un nemico
        bool spawnObstacle = Random.value < obstacleChance;
        Object soData = spawnObstacle ? obstacleSO : enemySO;

        if (soData == null || soData.prefab == null) return;

        float y = Random.Range(-yRange, yRange);
        float x = _player.position.x + spawnOffsetX;

        GameObject go = ObjectPooler.Instance.Spawn(soData, new Vector3(x, y, 0f), Quaternion.identity);

    }
}

