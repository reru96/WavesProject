using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("ScriptableObject Prefabs")]
<<<<<<< Updated upstream
    public Object enemySO;
    public Object obstacleSO;
=======
    public ObjectSO enemySO;
    public ObjectSO obstacleSO;
    public ObjectSO enemySpecialSO;
>>>>>>> Stashed changes

    [Header("Spawn Settings")]
    public float spawnRate = 2f;
    public float yRange = 4f;             
    public float xRange = 6f;             
    public float spawnOffset = 10f;     
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

    private void OnDestroy()
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

    private (Vector3 spawnPos, Vector2 moveDir) RandomDirection()
    {
<<<<<<< Updated upstream
        bool spawnObstacle = Random.value < obstacleChance;
        Object soData = spawnObstacle ? obstacleSO : enemySO;

        if (soData == null || soData.prefab == null) return;

=======
>>>>>>> Stashed changes
        Vector3 spawnPos = Vector3.zero;

        int dir = Random.Range(0, 4);

        switch (dir)
        {
            case 0: 
                spawnPos = new Vector3(_player.position.x + Random.Range(-xRange, xRange),
                                       _player.position.y + yRange + spawnOffset,
                                       0f);
                break;
<<<<<<< Updated upstream
            case 1: 
=======

            case 1: // Da giù verso su
>>>>>>> Stashed changes
                spawnPos = new Vector3(_player.position.x + Random.Range(-xRange, xRange),
                                       _player.position.y - yRange - spawnOffset,
                                       0f);
                break;
<<<<<<< Updated upstream
            case 2: 
=======

            case 2: // Da Destra verso Sinistra
>>>>>>> Stashed changes
                spawnPos = new Vector3(_player.position.x + xRange + spawnOffset,
                                       _player.position.y + Random.Range(-yRange, yRange),
                                       0f);
                break;
<<<<<<< Updated upstream
            case 3:
=======

            case 3: // Da Sinistra verso Destra
>>>>>>> Stashed changes
                spawnPos = new Vector3(_player.position.x - xRange - spawnOffset,
                                       _player.position.y + Random.Range(-yRange, yRange),
                                       0f);
                break;
        }

        return (spawnPos, moveDir);
    }

    private void SpawnEntity()
    {
        bool spawnObstacle = Random.value < obstacleChance;
        ObjectSO soData = spawnObstacle ? obstacleSO : enemySO;

        if (soData == null || soData.prefab == null) return;

        var (spawnPos, moveDir) = RandomDirection();

        GameObject go = ObjectPooler.Instance.Spawn(soData, spawnPos, Quaternion.identity);
    }

    public void SpawnSpecialEnemy()
    {
        var (spawnPos, moveDir) = RandomDirection();

        GameObject go = ObjectPooler.Instance.Spawn(enemySpecialSO, spawnPos, Quaternion.identity);

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
            enemy.SetDirection(moveDir);
    }
}

