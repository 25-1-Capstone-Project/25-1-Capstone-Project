using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MonsterAgent : Agent
{
    Rigidbody2D rBody;
    public Transform Target;
    public Transform[] Obstacles;
    public float forceMultiplier = 6f;
    public float stepPenalty = -0.001f;
    public float mapHalfSize = 5f;
    float prevDistance;

    void Start() => rBody = GetComponent<Rigidbody2D>();

    public override void OnEpisodeBegin()
    {
        rBody.linearVelocity = Vector2.zero;
        transform.position = new Vector3(0, 3f, 0);

        float minDistance = 1.5f;

        // 타깃 재배치
        Target.position = GetRandomPosition();

        // 장애물 재배치
        for (int i = 0; i < Obstacles.Length; i++)
        {
            Vector3 pos;
            int attempts = 0;
            do
            {
                pos = GetRandomPosition();
                attempts++;
            } while (
                (Vector2.Distance(pos, Target.position) < minDistance ||
                 Vector2.Distance(pos, transform.position) < minDistance ||
                 IsTooCloseToOtherObstacles(pos, Obstacles, i, minDistance))
                 && attempts < 100);

            Obstacles[i].position = pos;
        }

        prevDistance = Vector2.Distance(transform.position, Target.position);
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(-mapHalfSize + 0.5f, mapHalfSize - 0.5f),
            Random.Range(-mapHalfSize + 0.5f, mapHalfSize - 0.5f),
            0
        );
    }

    private bool IsTooCloseToOtherObstacles(Vector3 pos, Transform[] obstacles, int currentIndex, float minDistance)
    {
        for (int j = 0; j < currentIndex; j++)
        {
            if (Vector2.Distance(pos, obstacles[j].position) < minDistance)
                return true;
        }
        return false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 타깃 방향 + 거리
        Vector2 toTarget = Target.position - transform.position;
        sensor.AddObservation(toTarget.normalized);
        sensor.AddObservation(toTarget.magnitude / mapHalfSize);

        // 속도
        sensor.AddObservation(rBody.linearVelocity / 10f);

        // 장애물 정보 (방향 + 거리)
        foreach (var obs in Obstacles)
        {
            Vector2 toObs = (Vector2)(obs.position - transform.position);
            sensor.AddObservation(toObs.normalized);
            sensor.AddObservation(toObs.magnitude / mapHalfSize);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
{
    Vector2 move = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);
    rBody.AddForce(move * forceMultiplier);

    float distanceToTarget = Vector2.Distance(transform.position, Target.position);
    float distanceDelta = prevDistance - distanceToTarget;
    prevDistance = distanceToTarget;

    // 🔹 타겟 접근 보상
    AddReward(distanceDelta * 0.3f);

    // 🔹 장애물 근접 페널티 & 반발력
    float minObstacleDist = float.MaxValue;
    foreach (var obs in Obstacles)
    {
        Vector2 dir = (Vector2)(transform.position - obs.position);
        float d = dir.magnitude;
        if (d < minObstacleDist) minObstacleDist = d;

        if (d < 1.2f)
        {
            // 장애물 가까우면 페널티 강화
            AddReward(-0.03f / Mathf.Max(d, 0.3f));

            // 물리적 반발력 적용
            Vector2 avoidForce = dir.normalized * (1.2f - d) * forceMultiplier * 0.6f;
            rBody.AddForce(avoidForce, ForceMode2D.Force);
        }
    }

    // 🔹 장애물로부터 멀어지는 방향이면 소량 보상
    if (minObstacleDist > 1.0f)
        AddReward(0.002f);

    // 🔹 목표 도달
    if (distanceToTarget < 0.6f)
    {
        AddReward(+5.0f);
        EndEpisode();
        return;
    }

    // 🔹 장애물 충돌 (너무 가까움)
    foreach (var obs in Obstacles)
    {
        if (Vector2.Distance(transform.position, obs.position) < 0.4f)
        {
            AddReward(-3.0f);
            EndEpisode();
            return;
        }
    }

    // 🔹 맵 이탈
    if (Mathf.Abs(transform.position.x) > mapHalfSize || Mathf.Abs(transform.position.y) > mapHalfSize)
    {
        AddReward(-1.0f);
        EndEpisode();
        return;
    }

    // 🔹 비비기 감지: 속도 거의 없고 거리 변화도 없을 때
    if (StepCount % 100 == 0)
    {
        bool stuck = rBody.linearVelocity.magnitude < 0.05f && Mathf.Abs(distanceDelta) < 0.001f;
        if (stuck)
        {
            AddReward(-0.8f);
            EndEpisode();
            return;
        }
    }

    // 🔹 스텝 패널티
    AddReward(stepPenalty);
}


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;
        c[0] = Input.GetAxis("Horizontal");
        c[1] = Input.GetAxis("Vertical");
    }
}
