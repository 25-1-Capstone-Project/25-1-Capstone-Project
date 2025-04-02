using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public static PlayerTracker Instance; // 싱글톤 패턴으로 관리

    private Dictionary<string, int> playerActions = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RecordAction(string action)
    {
        if (playerActions.ContainsKey(action))
            playerActions[action]++;
        else
            playerActions[action] = 1;

        Debug.Log($"Action Recorded: {action} | Count: {playerActions[action]}");
    }

    public int GetActionCount(string action)
    {
        return playerActions.ContainsKey(action) ? playerActions[action] : 0;
    }

    public Dictionary<string, int> GetAllActions()
    {
        return playerActions;
    }
}
