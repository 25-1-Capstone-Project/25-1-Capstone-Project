using UnityEngine;

public enum SkillAction { Slash, Shot, AreaAttack, JumpSmash }

public class Fuzzy
{
    // Gaussian 멤버십 함수
    private float Gaussian(float x, float center, float sigma)
    {
        return Mathf.Exp(-Mathf.Pow(x - center, 2f) / (2f * Mathf.Pow(sigma, 2f)));
    }

    /// <summary>
    /// 거리, 보스 체력, 플레이어 체력, 플레이어 속도 기반으로 스킬 결정
    /// </summary>
    public SkillAction DecideSkillFuzzy(float distance, float hp, float playerHp, float playerVelocity)
    {
        // 1. 거리 멤버십 함수
        float distNear = Gaussian(distance, 1f, 1f);
        float distMid = Gaussian(distance, 4f, 1.5f);
        float distFar = Gaussian(distance, 8f, 2f);

        // 2. 보스 체력 멤버십 함수
        float hpLow = Gaussian(hp, 20f, 10f);
        float hpMed = Gaussian(hp, 50f, 10f);
        float hpHigh = Gaussian(hp, 80f, 10f);

        // 3. 플레이어 체력 멤버십 함수
        float playerWeak = Gaussian(playerHp, 20f, 10f);
        float playerNormal = Gaussian(playerHp, 50f, 10f);
        float playerStrong = Gaussian(playerHp, 80f, 10f);

        // 4. 플레이어 속도 멤버십 함수
        float playerSlow = Gaussian(playerVelocity, 0.5f, 0.7f);
        float playerMedium = Gaussian(playerVelocity, 2.5f, 1.0f);
        float playerFast = Gaussian(playerVelocity, 5f, 1.5f);

        // 5. 퍼지 규칙 평가 (Min 연산 + 우선순위 가중치)
        float scoreSlash = Mathf.Min(distNear, hpHigh, playerWeak, playerSlow) * 4f;
        float scoreShot = Mathf.Min(distFar, hpHigh, playerStrong, playerFast) * 3f;
        float scoreAOE = Mathf.Min(distMid, hpLow, playerStrong, playerFast) * 3.5f;
        float scoreJumpSmash = Mathf.Min(distNear, hpLow, playerWeak, playerMedium) * 5f;

        // 6. Defuzzification
        float totalScore = scoreSlash + scoreShot + scoreAOE + scoreJumpSmash + 0.0001f;

        float weightedAverage = (
            scoreSlash * 5f +
            scoreShot * 3f +
            scoreAOE * 4f +
            scoreJumpSmash * 6f
        ) / totalScore;

        // 7. 최종 스킬 선택
        if (weightedAverage >= 5.5f) return SkillAction.JumpSmash;
        if (weightedAverage >= 4f) return SkillAction.AreaAttack;
        if (weightedAverage >= 2.5f) return SkillAction.Shot;
        return SkillAction.Slash;
    }
}
