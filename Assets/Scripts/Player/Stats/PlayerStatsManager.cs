using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public PlayerData baseData; // ScriptableObject 참조
    public PlayerRuntimeStats runtimeStats;

    void Start()
    {
        if (!PlayerStatsSaveSystem.Load(out runtimeStats))
        {
            Debug.Log("파일이 없어 기본값으로 생성합니다.");
            runtimeStats = new PlayerRuntimeStats();
            runtimeStats.ApplyBase(baseData);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayerStatsSaveSystem.Save(runtimeStats);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (PlayerStatsSaveSystem.Load(out PlayerRuntimeStats loaded))
            {
                runtimeStats = loaded;
                Debug.Log("불러오기 완료 - 현재 체력: " + runtimeStats.currentHealth);
            }
        }
    }
}
