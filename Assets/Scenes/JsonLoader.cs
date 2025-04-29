using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerStats
{
    public float hp;                             // 현재 체력
    public float atk;                            // 공격력
    public float def;                            // 방어력 (받는 피해 감소율)
    public float moveSpeed;                      // 이동 속도
    public float attackSpeed;                    // 공격 속도
    public float dashSpeed;                      // 대시(회피) 속도

    public float parryWindow;                    // 패링 타이밍 관용도 (시간 여유)
    public float critChance;                     // 크리티컬 확률
    public float critDamage;                     // 크리티컬 발생 시 피해 배율

    public string[] activeSkills;                // 장착 중인 스킬들
    public int gold;                             // 현재 보유한 골드
    public string[] unlockedRunes;               // 해금한 룬 목록
    public string[] unlockedAreas;               // 해금한 지역 목록
    public string[] perks;                       // 획득한 패시브 능력
    public float playTime;                       // 누적 플레이 시간 (초 단위)

    public int comboLevel;                       // 콤보 단계 (공격 연계 수치) -> 없으면 빼기
    public int hitStreak;                        // 연속으로 공격 적중한 횟수 -> 없으면 빼기
    public string lastCheckpoint;               // 마지막 체크포인트 위치
}


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
