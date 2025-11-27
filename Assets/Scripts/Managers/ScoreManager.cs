using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private SaveData saveData;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        saveData = SaveManager.Load();
        if (saveData.highScores == null)
            saveData.highScores = new List<int>();
    }

    private void Start()
    {
        SetCanvas(0f, false);
    }

    public void AddScore(int newScore)
    {
        saveData.highScores.Add(newScore);

        saveData.highScores.Sort((a, b) => b.CompareTo(a));
        if (saveData.highScores.Count > 5)
            saveData.highScores.RemoveRange(5, saveData.highScores.Count - 5);

        SaveManager.Save(saveData);
    }

    public void SetCanvas(float alpha, bool first)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = first;
        canvasGroup.interactable = first;
    }

    public void OnEnable()
    {
        RespawnManager.Instance.OnGameOver += () => SetCanvas(1f, true);
    }
    public void OnDisable()
    {
        RespawnManager.Instance.OnGameOver -= () => SetCanvas(1f, true);
    }
    public List<int> GetHighScores()
    {
        return new List<int>(saveData.highScores);
    }
}
