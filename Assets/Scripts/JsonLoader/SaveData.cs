using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float hp;
    public float atk;
    public float def;
    public float moveSpeed;
    public float attackSpeed;
    public float dashSpeed;
    public float parryWindow;
    public float critChance;
    public float critDamage;
    public string[] activeSkills;
    public int gold;
    public string[] unlockedRunes;
    public string[] unlockedAreas;
    public string[] perks;
    public float playTime;
    public string lastCheckpoint;
    public int totalDeaths;       // 누적 사망 횟수
}