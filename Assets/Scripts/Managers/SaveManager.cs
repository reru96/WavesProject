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
    internal int sceneIndex;
    int score;
}