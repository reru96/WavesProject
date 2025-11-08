using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : Singleton<RespawnManager> 
{
    [SerializeField] private int maxTry = 3;
    private int leftTry;

    public int LeftTry => leftTry;
    public int MaxTry => maxTry;

    [SerializeField] private Transform puntoRespawn;
    [SerializeField] private float respawnDelay = 2f;

    UpdateLivesUI livesUI;

    private GameObject player;

    public GameObject GetPlayer() => player;

    public event Action OnPlayerReady;

    protected override bool ShouldBeDestroyOnLoad() => false;

    protected override void Awake()
    {
        base.Awake();
        FindPlayer();
        leftTry = maxTry;
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
        FindPlayer();
        livesUI.UpdateLives(); 
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            OnPlayerReady?.Invoke();
    }

    public void ResetTries()
    {
        leftTry = maxTry;
    }

    public void PlayerDied()
    {
        leftTry--;

        livesUI.UpdateLives();

        if (leftTry > 0)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            GameOver();
        }
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
        if (life != null) life.SetHp(life.GetMaxHp());

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
            SceneManager.LoadScene("StartMenu");
            ScreenFader.Instance.FadeIn();
        });
    }
}
