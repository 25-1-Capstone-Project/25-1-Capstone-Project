using UnityEngine;
using System.IO;

public static class PlayerStatsSaveSystem
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "playerStats.json");

    public static void Save(PlayerRuntimeStats stats)
    {
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(FilePath, json);
        Debug.Log("플레이어 스탯 저장됨: " + FilePath);
    }

    public static bool Load(out PlayerRuntimeStats loadedStats)
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            loadedStats = JsonUtility.FromJson<PlayerRuntimeStats>(json);
            Debug.Log("플레이어 스탯 불러옴");
            return true;
        }
        else
        {
            loadedStats = null;
            Debug.LogWarning("저장 파일이 없습니다");
            return false;
        }
    }
}
