using UnityEngine;
using System.Collections.Generic;

public class Fuzzy
{
    public List<FuzzyRule> rules;
    public ScalerData scaler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Fuzzy()
    {
        // Resources 폴더에서 텍스트 파일로 불러오기
        TextAsset rulesJson = Resources.Load<TextAsset>("fuzzy_rules");
        TextAsset scalerJson = Resources.Load<TextAsset>("fuzzy_scaler");

        // JSON -> 객체 변환 (Newtonsoft.Json 또는 JsonUtility 사용)
        rules = JsonUtility.FromJson<FuzzyRuleList>(rulesJson.text).rules;
        scaler = JsonUtility.FromJson<ScalerData>(scalerJson.text);
    }

    float[] NormalizeInput(float[] inputs, ScalerData scaler)
    {
        float[] norm = new float[inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            norm[i] = (inputs[i] - scaler.min[i]) / (scaler.max[i] - scaler.min[i]);
            norm[i] = Mathf.Clamp01(norm[i]); // 0~1 범위로 제한
        }
        return norm;
    }

public int PredictSkill(float hp, float playerHp, float distance, float playerVelocity)
{
    float[] input = new float[] { hp, playerHp, distance, playerVelocity };
    float[] inputNorm = NormalizeInput(input, scaler);

    float weightedSum = 0f;
    float totalWeight = 0f;

    foreach (var rule in rules)
    {
        // 유클리드 거리 계산
        float dist = 0f;
        for (int i = 0; i < inputNorm.Length; i++)
        {
            float diff = inputNorm[i] - rule.center[i];
            dist += diff * diff;
        }
        dist = Mathf.Sqrt(dist);

        // 가중치 (활성도) 계산 α
        float alpha = 1f / (1f + dist);

        // 회귀식 계산 z = coeffs·inputNorm + intercept
        float z = rule.intercept;
        for (int i = 0; i < inputNorm.Length; i++)
        {
            z += rule.coeffs[i] * inputNorm[i];
        }

        weightedSum += alpha * z;
        totalWeight += alpha;
    }

    if (totalWeight == 0)
        return 0;

    float skillScore = weightedSum / totalWeight;
    skillScore = Mathf.Clamp(skillScore, 0, 3);

    return Mathf.RoundToInt(skillScore);
}

}
