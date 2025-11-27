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
        HideCanvas();
    }

    public void AddScore(int newScore)
    {
        saveData.highScores.Add(newScore);

        saveData.highScores.Sort((a, b) => b.CompareTo(a));
        if (saveData.highScores.Count > 5)
            saveData.highScores.RemoveRange(5, saveData.highScores.Count - 5);

        SaveManager.Save(saveData);
    }

    public void HideCanvas()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
    public void ShowCanvas()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void OnEnable()
    {
        RespawnManager.Instance.OnGameOver += ShowCanvas;
    }                                        
    public void OnDestroy()                   
    {                                         
        RespawnManager.Instance.OnGameOver -= ShowCanvas;
    }
    public List<int> GetHighScores()
    {
        return new List<int>(saveData.highScores);
    }
}
