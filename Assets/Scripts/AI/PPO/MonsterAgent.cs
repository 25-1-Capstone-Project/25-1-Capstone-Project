using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MonsterAgent : Agent
{
    Rigidbody2D rBody;
    public Transform Target;
    public float forceMultiplier = 10f;
    public float stepPenalty = -0.001f;
    public float mapHalfSize = 5f;
    float prevDistance;

    [Header("Ray Sensor Settings")]
    public int rayCount = 16;              // 360도 감지할 Ray 개수
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
        Target.position = GetRandomPosition();
        prevDistance = Vector2.Distance(transform.position, Target.position);

        // 장애물 자동 랜덤 배치
        foreach (var obs in GameObject.FindGameObjectsWithTag("Wall"))
        {
            obs.transform.position = GetRandomPosition();
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(-mapHalfSize + 0.5f, mapHalfSize - 0.5f),
            Random.Range(-mapHalfSize + 0.5f, mapHalfSize - 0.5f),
            0
        );
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
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);


        float distanceToTarget = Vector2.Distance(transform.position, Target.position);
        float distanceDelta = prevDistance - distanceToTarget;
        prevDistance = distanceToTarget;

        // 🔹 타겟 접근 보상
        AddReward(distanceDelta * 0.3f);

        // 🔹 목표 도달
        if (distanceToTarget < 0.6f)
        {
            AddReward(+5.0f);
            EndEpisode();
            return;
        }

        // 🔹 장애물 충돌 감지
        if (Physics2D.OverlapCircle(transform.position, 0.3f, obstacleLayer))
        {
            AddReward(-3.0f);
            EndEpisode();
            return;
        }

        // 🔹 맵 이탈
        if (Mathf.Abs(transform.position.x) > mapHalfSize || Mathf.Abs(transform.position.y) > mapHalfSize)
        {
            AddReward(-1.0f);
            EndEpisode();
            return;
        }

        // 🔹 비비기 감지
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
