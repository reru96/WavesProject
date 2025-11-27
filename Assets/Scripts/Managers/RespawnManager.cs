using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : Singleton<RespawnManager>
{
    [Header("Lives")]
    [SerializeField] private int maxTry = 3;
    private int leftTry;

    [Header("Player Setup")]
    [SerializeField] private CreatureSO playerSO;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float respawnDelay = 2f;

    private GameObject player;

    public static event Action OnGameOver;
    public event Action<int> OnLivesChanged;
    public event Action<GameObject> OnPlayerSpawned;

    public GameObject Player => player;
    public int LeftTry => leftTry;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);  

        ResetTries();
        SceneManager.sceneLoaded += HandleSceneLoaded;
        SpawnPlayer();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnPlayer();
        NotifyLivesChanged();
    }

    private void SpawnPlayer()
    {
        if (playerSO == null || playerSO.prefab == null || spawnPoint == null) return;

        if (player != null) Destroy(player);

        player = Instantiate(playerSO.prefab, spawnPoint.position, Quaternion.identity);
        OnPlayerSpawned?.Invoke(player);
    }

    public void NotifyLivesChanged()
    {
        OnLivesChanged?.Invoke(leftTry);
    }

    public void ResetTries()
    {
        leftTry = maxTry;
        NotifyLivesChanged();
    }

    public void PlayerDied()
    {
        leftTry--;
        NotifyLivesChanged();

        if (leftTry > 0)
            StartCoroutine(RespawnRoutine());
        else
            GameOver();
    }

    private IEnumerator RespawnRoutine()
    {
        if (player == null) yield break;

        player.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);

        var life = player.GetComponent<LifeController>();
        if (life != null) life.SetHp(life.GetMaxHp());

        player.transform.position = spawnPoint.position;
        player.SetActive(true);

        OnPlayerSpawned?.Invoke(player);
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
        ResetTries();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }
}