using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : Singleton<RespawnManager>
{
    [Header("Player Setup")]
    [SerializeField] private CreatureSO playerSO;
    [SerializeField] private string levelSceneName = "Level1"; 
    private GameObject player;
    public static event Action OnGameOver;
    public event Action OnPlayerReady;
    public event Action<GameObject> OnPlayerSpawned; 

    public GameObject Player => player;


    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }


    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == levelSceneName)
        {
            Transform spawnPoint = FindSpawnPointInScene();

            SpawnPlayer(spawnPoint);
        }
        else
        {
            if (player != null)
            {
                Destroy(player);
                player = null;
            }
        }
    }

    private Transform FindSpawnPointInScene()
    {
        GameObject spawnGO = GameObject.FindWithTag("SpawnPoint");
        return spawnGO != null ? spawnGO.transform : null;
    }

    private void SpawnPlayer(Transform spawnPoint)
    {
        if (playerSO == null || playerSO.prefab == null || spawnPoint == null)
        {
            Debug.LogError("Impossibile spawnare il Player: mancano SO, Prefab o SpawnPoint nella scena.");
            return;
        }

        if (player != null) Destroy(player);

        player = Instantiate(playerSO.prefab, spawnPoint.position, Quaternion.identity);

        OnPlayerSpawned?.Invoke(player);
        OnPlayerReady?.Invoke();
    }


    public void PlayerDied()
    {
        GameOver();
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
        Debug.Log("Game Over! L'evento è stato notificato.");
    }
}