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
    public int comboLevel;
    public int hitStreak;
    public string lastCheckpoint;
}