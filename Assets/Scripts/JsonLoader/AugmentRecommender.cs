using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Newtonsoft.Json;
using System.Linq;

public class AugmentRecommender : MonoBehaviour
{
    [System.Serializable]
    public class AugmentModel
    {
        public List<string> features;
        public List<float> scaler_min;
        public List<float> scaler_max;
        public List<List<float>> cluster_centers;
        public Dictionary<string, List<string>> recommendations;
    }

    private AugmentModel model;
    private DatabaseReference dbRef;

    void Start()
    {
        LoadModel();

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            LoadPlayerLogAndRecommend("u001");
        });
    }

    void LoadModel()
    {
        TextAsset json = Resources.Load<TextAsset>("augment_model");
        model = JsonConvert.DeserializeObject<AugmentModel>(json.text);
    }

    void LoadPlayerLogAndRecommend(string userId)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("player_logs")
            .Child(userId)
            .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || !task.Result.Exists)
            {
                Debug.LogWarning("플레이어 로그를 가져올 수 없음");
                return;
            }

            Dictionary<string, float> playerLog = new Dictionary<string, float>();

            foreach (var child in task.Result.Children)
            {
                if (float.TryParse(child.Value.ToString(), out float val))
                {
                    playerLog[child.Key] = val;
                }
            }

            List<string> recommended = RecommendAugments(playerLog);
            Debug.Log($"[추천 증강 - {userId}] : " + string.Join(", ", recommended));
        });
    }

    List<string> RecommendAugments(Dictionary<string, float> playerLog)
    {
        float[] input = new float[model.features.Count];
        for (int i = 0; i < model.features.Count; i++)
        {
            string feature = model.features[i];
            float raw = playerLog.ContainsKey(feature) ? playerLog[feature] : 0f;
            float min = model.scaler_min[i];
            float max = model.scaler_max[i];
            input[i] = Mathf.Clamp(1f + 9f * (raw - min) / (max - min), 1f, 10f);
        }

        int bestCluster = 0;
        float minDistance = float.MaxValue;
        for (int c = 0; c < model.cluster_centers.Count; c++)
        {
            float dist = 0f;
            for (int i = 0; i < input.Length; i++)
            {
                float diff = input[i] - model.cluster_centers[c][i];
                dist += diff * diff;
            }
            if (dist < minDistance)
            {
                minDistance = dist;
                bestCluster = c;
            }
        }

        string key = bestCluster.ToString();
        return model.recommendations.ContainsKey(key) ? model.recommendations[key] : new List<string>();
    }
}
