using UnityEngine;

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