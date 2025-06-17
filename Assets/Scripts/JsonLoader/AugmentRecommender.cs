using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

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

    public List<AbilityChoiceUI> abilityUIList;

    public async void Run()
    {
        Debug.Log("📦 AugmentRecommender.Run() 실행됨");

        LoadModel();

        try
        {
            FirebaseDatabase db = FirebaseDatabase.GetInstance("https://capstone-project-2a41b-default-rtdb.asia-southeast1.firebasedatabase.app/");
            dbRef = db.RootReference;

            string userId = SystemInfo.deviceUniqueIdentifier;
            Debug.Log($"📡 [{userId}] 유저의 로그 데이터를 Firebase에서 불러옵니다...");

            var snapshot = await dbRef.Child("logs").Child(userId).GetValueAsync();

            if (!snapshot.Exists)
            {
                Debug.LogWarning($"⚠️ 유저 로그 없음: {userId}");
                return;
            }

            Debug.Log($"✅ 유저 로그 로드 완료!");

            Dictionary<string, float> playerLog = new();
            foreach (var child in snapshot.Children)
            {
                if (float.TryParse(child.Value.ToString(), out float val))
                    playerLog[child.Key] = val;
            }

            Debug.Log($"📊 파싱된 로그 수: {playerLog.Count}");

            var recommended = RecommendAugments(playerLog);
            Debug.Log($"🎯 추천 증강: {string.Join(", ", recommended)}");

            ApplyRecommendedToUI(recommended);
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Firebase 통신 오류: {ex.Message}");
        }
    }

    void LoadModel()
    {
        TextAsset json = Resources.Load<TextAsset>("augment_model");
        if (json == null)
        {
            Debug.LogError("🚫 augment_model.json 파일을 Resources 폴더에 넣으세요.");
            return;
        }

        model = JsonConvert.DeserializeObject<AugmentModel>(json.text);
        Debug.Log($"✅ 모델 로드 성공! feature 수: {model.features.Count}");
    }

    List<string> RecommendAugments(Dictionary<string, float> playerLog)
    {
        float[] input = new float[model.features.Count];
        for (int i = 0; i < model.features.Count; i++)
        {
            string feature = model.features[i];
            float val = playerLog.ContainsKey(feature) ? playerLog[feature] : 0f;
            float scaled = Mathf.Clamp(1f + 9f * (val - model.scaler_min[i]) / (model.scaler_max[i] - model.scaler_min[i]), 1f, 10f);
            input[i] = scaled;
        }

        int bestCluster = 0;
        float minDist = float.MaxValue;
        for (int c = 0; c < model.cluster_centers.Count; c++)
        {
            float dist = 0f;
            var center = model.cluster_centers[c];
            for (int i = 0; i < input.Length; i++)
            {
                float diff = input[i] - center[i];
                dist += diff * diff;
            }
            if (dist < minDist)
            {
                minDist = dist;
                bestCluster = c;
            }
        }

        string clusterKey = bestCluster.ToString();
        return model.recommendations.ContainsKey(clusterKey) ? model.recommendations[clusterKey] : new List<string>();
    }

    void ApplyRecommendedToUI(List<string> recommended)
    {
        foreach (var ui in abilityUIList)
        {
            var data = ui.GetAbilityData();
            if (data == null) continue;

            bool isRecommended = recommended.Contains(data.abilityName);
            ui.SetBlinking(isRecommended);
        }
    }
}
