using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private SaveData saveData;

    private void Awake()
    {
        saveData = SaveManager.Load();
        if (saveData.highScores == null)
            saveData.highScores = new List<int>();
    }

    public void AddScore(int newScore)
    {
        saveData.highScores.Add(newScore);

        saveData.highScores.Sort((a, b) => b.CompareTo(a));
        if (saveData.highScores.Count > 5)
            saveData.highScores.RemoveRange(5, saveData.highScores.Count - 5);

        SaveManager.Save(saveData);
    }

    public List<int> GetHighScores()
    {
        return new List<int>(saveData.highScores);
    }
}
