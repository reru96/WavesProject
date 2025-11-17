using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("ScriptableObject Prefabs")]
    public Object enemySO;
    public Object obstacleSO;

    [Header("Spawn Settings")]
    public float spawnRate = 2f;
    public float yRange = 4f;             
    public float xRange = 6f;             
    public float spawnOffset = 10f;     
    [Range(0f, 1f)] public float obstacleChance = 0.3f;

    [Header("EnumSettings")]
    public SceneType sceneType = SceneType.Simo;

    private float _timer;
    private Transform _player;
    public bool playerReady => _player != null;

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
        switch (sceneType)
        {
            case SceneType.Normal:
                _player = RespawnManager.Instance.GetPlayer().transform;
                break;
            case SceneType.Simo:
                Debug.Log("EnemySpawner: Getting player transform for Simo scene.");
                _player = RespawnManager.Instance.GetPlayer().GetComponentInChildren<SpriteFollower>().gameObject.transform;
                break;
        }
    }

    private void TryGetPlayer()
    {
        if (playerReady) return;

        Debug.Log("EnemySpawner: Trying to get player transform.");
        switch (sceneType)
        {
            case SceneType.Normal:
                _player = RespawnManager.Instance.GetPlayer().transform;
                break;
            case SceneType.Simo:
                _player = RespawnManager.Instance.GetPlayer().GetComponentInChildren<SpriteFollower>().gameObject.transform;
                break;
        }
    }


    void Update()
    {
        Debug.Log($"EnemySpawner: Player ready: {playerReady}");

        if (!playerReady) 
        {
            TryGetPlayer();
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= spawnRate)
        {
            _timer = 0f;
            SpawnEntity();
        }
    }

    private void SpawnEntity()
    {
        bool spawnObstacle = Random.value < obstacleChance;
        Object soData = spawnObstacle ? obstacleSO : enemySO;

        if (soData == null || soData.prefab == null) return;

        Vector3 spawnPos = Vector3.zero;

        int dir = Random.Range(0, 4);

        switch (dir)
        {
            case 0: 
                spawnPos = new Vector3(_player.position.x + Random.Range(-xRange, xRange),
                                       _player.position.y + yRange + spawnOffset,
                                       0f);
                break;
            case 1: 
                spawnPos = new Vector3(_player.position.x + Random.Range(-xRange, xRange),
                                       _player.position.y - yRange - spawnOffset,
                                       0f);
                break;
            case 2: 
                spawnPos = new Vector3(_player.position.x + xRange + spawnOffset,
                                       _player.position.y + Random.Range(-yRange, yRange),
                                       0f);
                break;
            case 3:
                spawnPos = new Vector3(_player.position.x - xRange - spawnOffset,
                                       _player.position.y + Random.Range(-yRange, yRange),
                                       0f);
                break;
        }

        GameObject go = ObjectPooler.Instance.Spawn(soData, spawnPos, Quaternion.identity);
    }
}

public enum SceneType
{
    Normal, 
    Simo
}

