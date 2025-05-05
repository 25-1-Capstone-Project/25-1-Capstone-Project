using UnityEngine;

public enum SkillAction { Idle, Slash, Dash, Shot, AreaAttack, JumpSmash }

public class Fuzzy
{
    // Gaussian 멤버십 함수
    private float Gaussian(float x, float c, float sigma)
    {
        return Mathf.Exp(-Mathf.Pow(x - c, 2f) / (2f * Mathf.Pow(sigma, 2f)));
    }

    /// <summary>
    /// 보스의 체력(hp)과 거리(distance)를 기준으로 사용할 스킬을 퍼지 논리로 결정합니다.
    /// </summary>
    public SkillAction DecideSkillFuzzy(float distance, float hp)
    {
        // 거리 멤버십 함수
        float near = Gaussian(distance, 1f, 1f);    // 근거리: 중심 1, 폭 1
        float mid = Gaussian(distance, 4f, 1f);     // 중거리: 중심 4
        float far = Gaussian(distance, 8f, 1.5f);   // 원거리: 중심 8

        // 체력 멤버십 함수
        float low = Gaussian(hp, 20f, 10f);         // 저체력
        float medium = Gaussian(hp, 50f, 10f);      // 중간체력
        float high = Gaussian(hp, 80f, 10f);        // 고체력

        // 각 스킬에 대한 퍼지 규칙 적용 (rule strength × weight)
        float slash = Mathf.Min(near, high) * 5f;         // 근거리 + 고체력
        float dash = Mathf.Min(mid, medium) * 4f;         // 중거리 + 중간체력
        float shot = Mathf.Min(far, high) * 3f;           // 원거리 + 고체력
        float aoe = Mathf.Min(mid, low) * 2f;             // 중거리 + 저체력
        float jumpSmash = Mathf.Min(near, low) * 1.5f;    // 근거리 + 저체력
        float idle = Mathf.Min(far, low) * 0f;            // 멀리 있고 체력 낮음 → 대기

        // 가중 평균 기반 defuzzification
        float total = slash + dash + shot + aoe + jumpSmash + idle + 0.0001f; // 0으로 나누는 것 방지

        float weightedAvg = (
            slash * 5f +
            dash * 4f +
            shot * 3f +
            aoe * 2f +
            jumpSmash * 1.5f +
            idle * 0f
        ) / total;

        // 출력 스킬 결정
        if (weightedAvg >= 4.5f) return SkillAction.Slash;
        if (weightedAvg >= 3.5f) return SkillAction.Dash;
        if (weightedAvg >= 2.5f) return SkillAction.Shot;
        if (weightedAvg >= 1.5f) return SkillAction.AreaAttack;
        if (weightedAvg >= 1.0f) return SkillAction.JumpSmash;
        return SkillAction.Idle;
    }
}
