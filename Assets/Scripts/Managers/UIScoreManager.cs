using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScoreManager : MonoBehaviour
{
    [Header("UI in-game")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI distanceText;
    public CanvasGroup leaderBoard;

    [Header("Score Settings")]
    public float pointsPerMeter = 1f;
    public float multiplierRate = 0.1f;

    [Header("Leaderboard")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private List<TMP_Text> scoreTexts;

    private Transform player;
    private float startX;
    private float distanceTravelled;
    private float timeAlive;
    private int score;
    private float multiplier = 1f;

    private void Start()
    {
        Hide();
        UpdateLeaderboard();

        if (RespawnManager.Instance != null && RespawnManager.Instance.Player != null)
            AssignPlayer(RespawnManager.Instance.Player);
    }

    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerSpawned += AssignPlayer;

        SaveController.OnScoreLoaded += SetScore;
        RespawnManager.OnGameOver += SaveAndUpdateLeaderboard;
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerSpawned -= AssignPlayer;

        SaveController.OnScoreLoaded -= SetScore;
        RespawnManager.OnGameOver -= SaveAndUpdateLeaderboard;
    }

    private void AssignPlayer(GameObject newPlayer)
    {
        player = newPlayer.transform;
        startX = player.position.x;
        timeAlive = 0f;
        score = 0;
    }

    private void Update()
    {
        if (player == null) return;

        distanceTravelled = Mathf.Max(0, player.position.x - startX);
        timeAlive += Time.deltaTime;
        multiplier = 1f + (timeAlive * multiplierRate);
        score = Mathf.FloorToInt(distanceTravelled * pointsPerMeter * multiplier);

        if (scoreText != null)
            scoreText.text = $": {score}\n: x{multiplier:F2}";
        if (distanceText != null)
            distanceText.text = $": {distanceTravelled:F0}m";
    }

    public void SetScore(int newScore)
    {
        score = newScore;
    }

    public void Hide()
    {
        leaderBoard.alpha = 0f;
        leaderBoard.interactable = false;
        leaderBoard.blocksRaycasts = false;
    }

    public void SaveAndUpdateLeaderboard()
    {
        scoreManager.AddScore(score);
        SaveData data = SaveManager.Load();
        data.score = score;
        SaveManager.Save(data);
        UpdateLeaderboard();

        Time.timeScale = 0f;
        leaderBoard.alpha = 1f;
        leaderBoard.interactable = true;
        leaderBoard.blocksRaycasts = true;
    }

    public void UpdateLeaderboard()
    {
        var scores = scoreManager.GetHighScores();
        for (int i = 0; i < scoreTexts.Count; i++)
            scoreTexts[i].text = i < scores.Count ? $"{i + 1}. {scores[i]}" : $"{i + 1}. ---";
    }
}