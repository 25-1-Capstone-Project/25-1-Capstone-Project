using UnityEngine;
using UnityEngine.UI;

public enum SkillAction { Idle, Slash, Dash, Shot}


public class Fuzzy
{ 
//가우시안 함수 계산
    float Gaussian(float x, float c, float sigma)
    {
        return Mathf.Exp(-Mathf.Pow(x - c, 2f) / (2f * Mathf.Pow(sigma, 2f)));
    }

    public SkillAction DecideSkillFuzzy(float distance, float hp)
    {
        float near = Gaussian(distance, 1f, 1f);
        float mid = Gaussian(distance, 4f, 1f);
        float far = Gaussian(distance, 8f, 1.5f);

        float low = Gaussian(hp, 20f, 10f);
        float medium = Gaussian(hp, 50f, 10f);
        float high = Gaussian(hp, 80f, 10f);

        Debug.Log($"[퍼지] near: {near:F2}, mid: {mid:F2}, far: {far:F2} | low: {low:F2}, medium: {medium:F2}, high: {high:F2}");

        float slashRule = Mathf.Min(near, high) * 4f;
        float dashRule = Mathf.Min(mid, medium) * 3f;
        float rangedRule = Mathf.Min(far, high) * 2f;
        float healRule = Mathf.Min(far, low) * 1f;

        float totalWeight = slashRule + dashRule + rangedRule + healRule + 0.0001f;
        
        float weightedAverage = (
            slashRule * 4f +
            dashRule * 3f +
            rangedRule * 2f +
            healRule * 1f
        ) / totalWeight;

        Debug.Log($"[퍼지] Weighted Avg: {weightedAverage:F2}");

        if (weightedAverage >= 4.5f) return SkillAction.Slash;
        if (weightedAverage >= 3.5f) return SkillAction.Dash;
        if (weightedAverage >= 2.5f) return SkillAction.Shot;

        return SkillAction.Idle;
    }
}
