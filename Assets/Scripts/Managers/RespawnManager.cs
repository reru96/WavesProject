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

    public event Action OnPlayerReady;
    public event Action OnLivesChanged;

    public int LeftTry => leftTry;
    public int MaxTry => maxTry;
    public GameObject Player => player;

    protected override void Awake()
    {
        base.Awake();
        leftTry = maxTry;
    }
    private void Start()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        SpawnPlayer();
        NotifyLivesChanged();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        NotifyLivesChanged();
    }

    private void SpawnPlayer()
    {
        if (playerSO == null || playerSO.prefab == null)
        {
            Debug.LogError("[RespawnManager] PlayerSO o Prefab mancante.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[RespawnManager] SpawnPoint mancante!");
            return;
        }

        player = Instantiate(playerSO.prefab, spawnPoint.position, Quaternion.identity);

        OnPlayerReady?.Invoke();
    }

    public void NotifyLivesChanged()
    {
        OnLivesChanged?.Invoke();
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

        bool fadeDone = false;
        ScreenFader.Instance.FadeOut(() => fadeDone = true);

        while (!fadeDone) yield return null;

        player.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        var life = player.GetComponent<LifeController>();
        if (life != null)
        {
            life.SetHp(life.GetMaxHp());
        }

        player.transform.position = spawnPoint.position;
        player.SetActive(true);

        fadeDone = false;
        ScreenFader.Instance.FadeIn(() => fadeDone = true);

        while (!fadeDone) yield return null;
    }

    private void GameOver()
    {
        ScreenFader.Instance.FadeOut(() =>
        {
            SceneManager.LoadScene("MainMenu");
            ScreenFader.Instance.FadeIn();
        });

        ResetTries();
    }
}
