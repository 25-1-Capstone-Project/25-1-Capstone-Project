using UnityEngine;
using System.Linq;
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

    Debug.Log($"[Fuzzy] 입력 (원본) HP={hp:F1}, PlayerHP={playerHp:F1}, Distance={distance:F2}, PlayerVel={playerVelocity:F2}");
    Debug.Log($"[Fuzzy] 입력 (정규화) [{string.Join(", ", inputNorm.Select(x => x.ToString("F3")))}]");


    float weightedSum = 0f;
    float totalWeight = 0f;

    // 가장 적합한 규칙을 기억할 변수들
    int bestRuleIndex = -1;
    float bestAlpha = -1f;
    float bestZ = 0f;
    float[] bestCoeffs = null;
    float bestIntercept = 0f;
    float[] bestCenter = null;


        for (int i = 0; i < rules.Count; i++)
        {
            var rule = rules[i];
            // 유클리드 거리 계산
            float dist = 0f;
            for (int j = 0; j < inputNorm.Length; j++)
            {
                float diff = inputNorm[j] - rule.center[j];
                dist += diff * diff;
            }
            dist = Mathf.Sqrt(dist);

            // 가중치 (활성도) 계산 α
            float alpha = 1f / (1f + dist);

            // 회귀식 계산 z = coeffs·inputNorm + intercept
            float z = rule.intercept;
            for (int j = 0; j < inputNorm.Length; j++)
            {
                z += rule.coeffs[j] * inputNorm[j];
            }

            weightedSum += alpha * z;
            totalWeight += alpha;
            
            if (alpha > bestAlpha)
        {
            bestAlpha = alpha;
            bestRuleIndex = i;
            bestZ = z;
            bestCoeffs = rule.coeffs;
            bestIntercept = rule.intercept;
            bestCenter = rule.center;
        }
        }

    if (totalWeight == 0)
        return 0;

    Debug.Log($"[Fuzzy] 최적 규칙 #{bestRuleIndex}: center=[{string.Join(", ", bestCenter)}], α={bestAlpha:F4}, z={bestZ:F4}");
    
    string formula = $"z = {bestIntercept:F3}";
    for (int j = 0; j < bestCoeffs.Length; j++)
        formula += $" + ({bestCoeffs[j]:F3}×{inputNorm[j]:F3})";
    Debug.Log($"[Fuzzy] 최적 규칙 회귀식: {formula}");


    float skillScore = weightedSum / totalWeight;
    skillScore = Mathf.Clamp(skillScore, 0, 3);

    return Mathf.RoundToInt(skillScore);
}

}
