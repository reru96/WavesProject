using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : Singleton<RespawnManager> 
{
    [Header("Player Settings")]
    [SerializeField] private int maxTry = 3;
    private int leftTry;
    public Object playerSO; // ScriptableObject del player

    public int LeftTry => leftTry;
    public int MaxTry => maxTry;

    [Header("Respawn Settings")]
    [SerializeField] private Transform puntoRespawn;
    [SerializeField] private float respawnDelay = 2f;

    private GameObject player;
    public GameObject GetPlayer() => player;

    public event Action OnPlayerReady;

    private UpdateLivesUI livesUI;

    protected override bool ShouldBeDestroyOnLoad() => false;

    protected override void Awake()
    {
        base.Awake();
        leftTry = maxTry;
        SpawnPlayer(); 
        livesUI = FindAnyObjectByType<UpdateLivesUI>(); 
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Rispawna il player se non c'è
        if (player == null)
            SpawnPlayer();

        livesUI?.UpdateLives();
    }

    private void SpawnPlayer()
    {
        if (playerSO == null || playerSO.prefab == null)
        {
            Debug.LogWarning("[RespawnManager] PlayerSO o prefab mancante!");
            return;
        }

        if (player == null)
        {
            player = Instantiate(playerSO.prefab, puntoRespawn.position, Quaternion.identity);
            OnPlayerReady?.Invoke();
        }
    }

    public void ResetTries()
    {
        leftTry = maxTry;
    }

    public void PlayerDied()
    {
        leftTry--;
        livesUI?.UpdateLives();

        if (leftTry > 0)
            StartCoroutine(RespawnRoutine());
        else
            GameOver();
    }

    private IEnumerator RespawnRoutine()
    {
        if (player == null) yield break;

        bool done = false;
        ScreenFader.Instance.FadeOut(() => done = true);
        while (!done) yield return null;

        player.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);

        var life = player.GetComponent<LifeController>();
        life?.SetHp(life.GetMaxHp());

        player.transform.position = puntoRespawn.position;
        player.SetActive(true);

        done = false;
        ScreenFader.Instance.FadeIn(() => done = true);
        while (!done) yield return null;
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        ResetTries();

        ScreenFader.Instance.FadeOut(() =>
        {
            SceneManager.LoadScene("MainMenu");
            ScreenFader.Instance.FadeIn();
        });
    }
}
