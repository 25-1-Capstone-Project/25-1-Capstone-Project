using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PlayerSaveManager : MonoBehaviour
{
    public Dictionary<string, SaveData> saveDataDict = new Dictionary<string, SaveData>();
    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerStats.json");

        // 샘플 데이터 만들기
        // 테스트용 초기 값 설정
        SaveData saveData = new SaveData();
        saveData.hp = 100f;
        saveData.atk = 12f;
        saveData.def = 5f;
        saveData.moveSpeed = 5f;
        saveData.attackSpeed = 1.2f;
        saveData.dashSpeed = 8f;
        saveData.parryWindow = 0.15f;
        saveData.critChance = 0.1f;
        saveData.critDamage = 1.5f;
        saveData.activeSkills = new string[] { "대시베기", "점프공격" };
        saveData.gold = 350;
        saveData.unlockedRunes = new string[] { "화염", "속도" };
        saveData.unlockedAreas = new string[] { "1-1", "1-2" };
        saveData.perks = new string[] { "이동속도 +10%", "치명타 +5%" };
        saveData.playTime = 123.5f;
        saveData.comboLevel = 2;
        saveData.hitStreak = 4;
        saveData.lastCheckpoint = "1-2 보스방 앞";

        // 딕셔너리에 저장
        saveDataDict["Save1"] = saveData;

        Save();
        Load("Save1");
    }

    [System.Serializable]
    class SaveWrapper
    {
        public List<string> keys = new List<string>();
        public List<SaveData> values = new List<SaveData>();
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
                SaveData loaded = saveDataDict[saveName];
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
