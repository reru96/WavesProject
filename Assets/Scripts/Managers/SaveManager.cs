using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public static class SaveManager
{
    private static string savePath = Application.persistentDataPath + "/save3.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public static SaveData Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return new SaveData();
        }
    }
}

[System.Serializable] public class SaveData
{
    public List<int> highScores = new List<int>();
    public int sceneIndex;
    public float playerX, playerY, playerZ;
    public int score;
    public int coins;
    public float musicVolume;
    public float sfxVolume;
}