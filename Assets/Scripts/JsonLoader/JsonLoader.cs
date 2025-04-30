using UnityEngine;
using System.IO;

public class PlayerSaveManager : MonoBehaviour
{
    public PlayerStats player = new PlayerStats();
    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerStats.json");
        Save();
   
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(player, true);
        File.WriteAllText(savePath, json);
        Debug.Log("✅ 저장 완료: " + savePath);
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            player = JsonUtility.FromJson<PlayerStats>(json);
            Debug.Log("✅ 불러오기 완료!");
            Debug.Log("플레이어 체력: " + player.hp);
        }
        else
        {
            Debug.LogWarning("⚠ 저장된 파일이 없습니다.");
        }
    }
}
