using UnityEngine;


public class Fuzzy
{
    private float Gaussian(float x, float center, float sigma)
    {
        return Mathf.Exp(-Mathf.Pow(x - center, 2f) / (2f * Mathf.Pow(sigma, 2f)));
    }

    /// <summary>
    /// Sugeno 방식으로 스킬 결정
    /// </summary>
    public EBossSkillAction DecideSkill(float distance, float hp, float playerHp, float playerVelocity)
    {
        // 1. 퍼지 멤버십 계산
        float distNear = Gaussian(distance, 1f, 1f);
        float distFar = Gaussian(distance, 8f, 2f);
        float distMid = Gaussian(distance, 4f, 1.5f);

        float hpLow = Gaussian(hp, 20f, 10f);
        float hpHigh = Gaussian(hp, 80f, 10f);

        float playerWeak = Gaussian(playerHp, 20f, 10f);
        float playerStrong = Gaussian(playerHp, 80f, 10f);

        float playerSlow = Gaussian(playerVelocity, 0.5f, 0.7f);
        float playerFast = Gaussian(playerVelocity, 5f, 1.5f);

        // 2. 규칙의 조건부 가중치 (Min 연산)
        float w1 = Mathf.Min(distNear, hpHigh, playerWeak, playerSlow);     // Rule 1: Slash
        float w2 = Mathf.Min(distFar, hpHigh, playerStrong, playerFast);    // Rule 2: Shot
        float w3 = Mathf.Min(distMid, hpLow, playerStrong, playerFast);     // Rule 3: AOE
        float w4 = Mathf.Min(distNear, hpLow, playerWeak, playerSlow);      // Rule 4: JumpSmash

        // 3. 각 규칙의 결론부 (Sugeno 함수: z = ax + by + ...)
        float z1 = 0.2f * distance + 0.1f * hp + 0.1f * playerHp + 0.2f * playerVelocity + 1f;  // Slash
        float z2 = 0.4f * distance + 0.2f * hp + 0.2f * playerHp + 0.3f * playerVelocity + 2f;  // Shot
        float z3 = 0.3f * distance + 0.1f * hp + 0.3f * playerHp + 0.4f * playerVelocity + 3f;  // AOE
        float z4 = 0.1f * distance + 0.2f * hp + 0.2f * playerHp + 0.1f * playerVelocity + 5f;  // JumpSmash

        // 4. Defuzzification (가중 평균)
        float numerator = w1 * z1 + w2 * z2 + w3 * z3 + w4 * z4;
        float denominator = w1 + w2 + w3 + w4 + 0.0001f;  // 0으로 나누는 것 방지
        float output = numerator / denominator;

        // 5. 출력값 기반 스킬 결정
        if (output > 4.5f) return EBossSkillAction.JumpSmash;
        if (output > 3.5f) return EBossSkillAction.AreaAttack;
        if (output > 2.5f) return EBossSkillAction.Shot;
        return EBossSkillAction.Slash;
    }
}
