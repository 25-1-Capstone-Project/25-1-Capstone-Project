using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int saveIndex;                     // 저장 회차 인덱스
    public float hp;                             // 현재 체력
    public int gold;                             // 현재 보유한 골드
    public int equipedRune;                    // 장착중인 룬  
    public bool[] unlockedRunes;               // 해금한 룬 목록
    public bool[] unlockedAreas;               // 해금한 지역 목록
    public string[] utilSkill;                  // 획득한 사신수 가호
    public float playTime;                       // 누적 플레이 시간 (초 단위)

    public string lastCheckpoint;               // 마지막 체크포인트 위치
}
