using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : Singleton<RespawnManager>
{
    [SerializeField] private int maxTry = 3;
    private int leftTry;
    public Object playerSO;
    public int LeftTry => leftTry;
    public int MaxTry => maxTry;

    [Header("Respawn Settings")]
    [SerializeField] private Transform puntoRespawn;
    [SerializeField] private float respawnDelay = 2f;

    private GameObject player;
    public GameObject GetPlayer() => player;

    public event Action OnPlayerReady;

    private UpdateLivesUI livesUI;

    private EnemySpawner enemySpawner;

    protected override bool ShouldBeDestroyOnLoad() => false;

    protected override void Awake()
    {
        base.Awake();
        leftTry = maxTry;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        SpawnPlayer();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    private void Update()
    {
        if (GetPlayer().GetComponent<SpriteFollower>() == null && enemySpawner.sceneType == SceneType.Simo)
        {
            player = GetPlayer().GetComponentInChildren<SpriteFollower>().gameObject;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        livesUI = FindAnyObjectByType<UpdateLivesUI>();
        NotifyLifeChanged();
    }

    private void SpawnPlayer()
    {
        if (puntoRespawn == null)
        {
            puntoRespawn = GetPlayer()?.transform;
            if (puntoRespawn == null)
            {
                Debug.LogWarning("[RespawnManager] Punto di respawn non assegnato e nessun player trovato nella scena.");
                return;
            }
        }

        if (playerSO == null || playerSO.prefab == null)
        {
            Debug.LogWarning("[RespawnManager] PlayerSO o prefab mancante.");
            return;
        }

        if (player == null)
        {
            player = Instantiate(playerSO.prefab, puntoRespawn.position, Quaternion.identity);  
            OnPlayerReady?.Invoke();
        }
    }

    public void NotifyLifeChanged()
    {
        if (livesUI == null)
            livesUI = FindAnyObjectByType<UpdateLivesUI>();

        livesUI?.UpdateLives();
    }

    public void ResetTries()
    {
        leftTry = maxTry;
        NotifyLifeChanged();
    }

    public void PlayerDied()
    {
        if(GetPlayer().GetComponent<LifeController>() != null && GetPlayer().GetComponent<LifeController>().isInvincible)        
            return;
        

        leftTry--;
        NotifyLifeChanged();

        if (leftTry > 0)
            StartCoroutine(RespawnRoutine());
        else
            GameOver();
    }

    private IEnumerator RespawnRoutine()
    {
        if (player == null) yield break;

        Debug.Log("Coroutine di respawn avviata.");

        bool done = false;
        ScreenFader.Instance.FadeOut(() => done = true);
        while (!done) yield return null;

        player.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);

        var life = player.GetComponent<LifeController>();
        if (life == null) life = player.GetComponentInChildren<LifeController>();
        if (life == null)
        {
            Debug.LogWarning("[RespawnManager] LifeController non trovato sul player.");
            leftTry = maxTry;
            yield break;
        }

        life?.SetHp(life.GetMaxHp());
        player.transform.position = puntoRespawn.position;
        player.SetActive(true);

        NotifyLifeChanged();

        done = false;
        ScreenFader.Instance.FadeIn(() => done = true);
        while (!done) yield return null;
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
