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

    private void Start()
    {
        Hide();
        UpdateLeaderboard();

        if (RespawnManager.Instance != null && RespawnManager.Instance.Player != null)
            AssignPlayer(RespawnManager.Instance.Player);
        else
        {
          var playerobj = FindAnyObjectByType<PlayerControl>();
            player = playerobj.transform;
        }
    }

    public void OnEnable()
    {
        SaveController.OnScoreLoaded += SetScore;
        RespawnManager.OnGameOver += SaveAndUpdateLeaderboard;
    }

    public void OnDisable()
    {
        SaveController.OnScoreLoaded -= SetScore;
        RespawnManager.OnGameOver -= SaveAndUpdateLeaderboard;
    }

    public void AssignPlayer(GameObject newPlayer)
    {
        player = newPlayer.transform;
        startX = player.position.x;
        timeAlive = 0f;
        score = 0;
    }

    public void Update()
    {
        if (player == null) return;

        distanceTravelled = Mathf.Max(0, player.position.x - startX);
        timeAlive += Time.deltaTime;
        score = Mathf.FloorToInt(distanceTravelled * pointsPerMeter);

        if (scoreText != null)
            scoreText.text = $"Score: {score}\n";
        if (distanceText != null)
            distanceText.text = $"Distance: {distanceTravelled:F0}m";
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