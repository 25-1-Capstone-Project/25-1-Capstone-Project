using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ChasingAgent : Agent
{
    Rigidbody2D rBody;
    public Transform Target;
    public float forceMultiplier = 10f;
    public float stepPenalty = -0.001f;
    public float mapHalfSize = 5f;
    float prevDistance;

    [Header("Ray Sensor Settings")]
    public int rayCount = 32;              // 360도 감지할 Ray 개수
    public float rayLength = 5f;           // 각 Ray의 길이
    public LayerMask obstacleLayer;        // 감지할 장애물 레이어

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
{
    rBody.linearVelocity = Vector2.zero;
    transform.position = new Vector3(0, 3f, 0);

    // 장애물 먼저 배치
    foreach (var obs in GameObject.FindGameObjectsWithTag("Wall"))
    {
        obs.transform.position = GetRandomPosition(1.5f);
    }

    // 타겟 배치 (장애물과 충분히 떨어져 있도록)
    Target.position = GetRandomPosition(2.0f);

    prevDistance = Vector2.Distance(transform.position, Target.position);
}

    private Vector3 GetRandomPosition(float minDistanceFromOthers = 1.0f)
{
    Vector3 pos;
    int safety = 0;

    do
    {
        pos = new Vector3(
            Random.Range(-mapHalfSize + 0.5f, mapHalfSize - 0.5f),
            Random.Range(-mapHalfSize + 0.5f, mapHalfSize - 0.5f),
            0
        );
        safety++;

        // 주변에 장애물이 너무 가깝지 않게
        bool tooClose = false;
        foreach (var obs in GameObject.FindGameObjectsWithTag("Wall"))
        {
            if (Vector3.Distance(obs.transform.position, pos) < minDistanceFromOthers)
            {
                tooClose = true;
                break;
            }
        }

        if (Vector3.Distance(transform.position, pos) < minDistanceFromOthers)
            tooClose = true; // Agent와 너무 가까워도 안 됨

        if (!tooClose)
            return pos;

    } while (safety < 100);

    Debug.LogWarning("GetValidRandomPosition: failed to find a valid spot!");
    return pos;
}

    public override void CollectObservations(VectorSensor sensor)
    {
        // 🔹 타겟 방향 + 거리
        Vector2 toTarget = Target.position - transform.position;
        sensor.AddObservation(toTarget.normalized);
        sensor.AddObservation(toTarget.magnitude / mapHalfSize);

        // 🔹 속도
        sensor.AddObservation(rBody.linearVelocity / 10f);

        // 🔹 360도 Ray 감지
        float angleStep = 360f / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = angleStep * i;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, rayLength, obstacleLayer);
            float distance = hit.collider ? hit.distance / rayLength : 1f; // 정규화 거리 (0~1)
            sensor.AddObservation(distance);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
{
    // 1️⃣ 이동
    Vector3 controlSignal = Vector3.zero;
    controlSignal.x = actionBuffers.ContinuousActions[0];
    controlSignal.y = actionBuffers.ContinuousActions[1];
    rBody.AddForce(controlSignal * forceMultiplier);

    // 2️⃣ 거리 계산
    float distanceToTarget = Vector2.Distance(transform.position, Target.position);
    float distanceDelta = prevDistance - distanceToTarget;
    prevDistance = distanceToTarget;

    // 🔹 타겟 접근 보상
    if (distanceDelta > 0)
        AddReward(distanceDelta * 0.3f);  // 가까워질수록 보상
    else
        AddReward(distanceDelta * 0.02f); // 멀어져도 패널티 최소화

    // 3️⃣ 목표 도달
    if (distanceToTarget < 0.6f)
    {
        AddReward(+5.0f);
        EndEpisode();
        return;
    }

    // 4️⃣ 장애물 충돌
    if (Physics2D.OverlapCircle(transform.position, 0.3f, obstacleLayer))
    {
        AddReward(-5.0f);
        EndEpisode();
        return;
    }

    // 5️⃣ 맵 이탈
    if (Mathf.Abs(transform.position.x) > mapHalfSize || Mathf.Abs(transform.position.y) > mapHalfSize)
    {
        AddReward(-2.0f);
        EndEpisode();
        return;
    }

    // 6️⃣ 장애물 근처에서 배회 방지
    Collider2D[] nearObstacles = Physics2D.OverlapCircleAll(transform.position, 1.0f, obstacleLayer);
    if (nearObstacles.Length > 0)
    {
        // 장애물 근처인데 이동 속도가 느리거나
        // 목표 방향과 이동 방향이 크게 어긋나면 소규모 패널티
        Vector2 moveDir = rBody.linearVelocity.normalized;
        Vector2 toTarget = ((Vector2)Target.position - (Vector2)transform.position).normalized;
        float alignment = Vector2.Dot(moveDir, toTarget); // 1: 목표 방향, -1: 반대 방향

        if (rBody.linearVelocity.magnitude < 0.05f || alignment < 0.5f)
        {
            AddReward(-0.02f); // 배회 방지 패널티
        }
        else
        {
            AddReward(+0.01f); // 올바른 우회 움직임은 소규모 보상
        }
    }

    // 7️⃣ 비비기 감지
    if (StepCount % 100 == 0)
    {
        bool stuck = rBody.linearVelocity.magnitude < 0.05f && Mathf.Abs(distanceDelta) < 0.001f;
        if (stuck)
        {
            AddReward(-1.0f);
            EndEpisode();
            return;
        }
    }

    // 8️⃣ 스텝 패널티
    AddReward(stepPenalty);
}



    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic called");

        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }


/*
#if UNITY_EDITOR
    // 🔹 Ray 시각화 (디버깅용)
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        float angleStep = 360f / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = angleStep * i;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Gizmos.DrawRay(transform.position, dir * rayLength);
        }
    }
#endif
*/

}
