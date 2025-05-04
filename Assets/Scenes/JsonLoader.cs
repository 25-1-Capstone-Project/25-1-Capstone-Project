using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PlayerSaveManager : MonoBehaviour
{
    public Dictionary<string, PlayerStats> saveDataDict = new Dictionary<string, PlayerStats>();
    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerStats.json");

        // 샘플 데이터 만들기
        // 테스트용 초기 값 설정
        PlayerStats player = new PlayerStats();
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

        // 딕셔너리에 저장
        saveDataDict["Save1"] = player;

        Save();
        Load("Save1");
    }

    [System.Serializable]
    class SaveWrapper
    {
        public List<string> keys = new List<string>();
        public List<PlayerStats> values = new List<PlayerStats>();
    }

    public void Save()
    {
        SaveWrapper wrapper = new SaveWrapper();
        foreach (var kv in saveDataDict)
        {
            wrapper.keys.Add(kv.Key);
            wrapper.values.Add(kv.Value);
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savePath, json);
        Debug.Log("저장 완료");
    }

    public void Load(string saveName)
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);

            saveDataDict.Clear();
            for (int i = 0; i < wrapper.keys.Count; i++)
            {
                saveDataDict[wrapper.keys[i]] = wrapper.values[i];
            }

            if (saveDataDict.ContainsKey(saveName))
            {
                PlayerStats loaded = saveDataDict[saveName];
                Debug.Log("불러오기 성공: " + saveName);
                Debug.Log("HP: " + loaded.hp);
            }
            else
            {
                Debug.LogWarning("슬롯 없음: " + saveName);
            }
        }
        else
        {
            Debug.LogWarning("저장 파일 없음");
        }
    }
}
