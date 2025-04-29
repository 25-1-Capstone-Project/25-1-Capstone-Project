using UnityEngine;
using System.IO;

public class PlayerSaveManager : MonoBehaviour
{
    public PlayerStats player = new PlayerStats();
    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerStats.json");

        // 테스트용 초기 값 설정
        player.hp = 100f;
        player.atk = 12f;
        player.def = 5f;
        player.moveSpeed = 5f;
        player.attackSpeed = 1.2f;
        player.dashSpeed = 8f;

        player.parryWindow = 0.15f;
        player.critChance = 0.1f;
        player.critDamage = 1.5f;

        player.activeSkills = new string[] { "대시베기", "점프공격" };
        player.gold = 350;
        player.unlockedRunes = new string[] { "화염", "속도" };
        player.unlockedAreas = new string[] { "1-1", "1-2" };
        player.perks = new string[] { "이동속도 +10%", "치명타 +5%" };
        player.playTime = 123.5f;

        player.comboLevel = 2;
        player.hitStreak = 4;
        player.lastCheckpoint = "1-2 보스방 앞";

        Save();
        Load();
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
