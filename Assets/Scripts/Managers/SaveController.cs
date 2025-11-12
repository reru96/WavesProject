using System;
using UnityEngine;

public class SaveController : Singleton<SaveController>
{
    public static SaveData pendingSaveData;
    public static event Action<int> OnScoreLoaded;

    public void Start()
    {
        LoadScore();
    }
    public void LoadScore()
    {
        OnScoreLoaded?.Invoke(pendingSaveData.score);
    }

    protected override bool ShouldBeDestroyOnLoad() => false;

    public void SaveGame()
    {
        SaveData data = SaveManager.Load();

        data.sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        SaveManager.Save(data);
    }
}
