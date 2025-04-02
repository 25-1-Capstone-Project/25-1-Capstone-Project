using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class EnemyAgent : Agent
{
    [Header("Enemy Properties")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float rayDistance = 5f;
    public LayerMask obstacleLayer;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    
    [Header("References")]
    public Transform playerTransform;
    
    private Rigidbody2D rb;
    private List<EnemyAgent> otherEnemies = new List<EnemyAgent>();
    
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();

        // 다른 적 에이전트 찾기
        EnemyAgent[] allEnemies = Object.FindObjectsOfType<EnemyAgent>();
        foreach (EnemyAgent enemy in allEnemies)
        {
            if (enemy != this)
            {
                otherEnemies.Add(enemy);
            }
        }
    }
    
    public override void OnEpisodeBegin()
    {
        // 에피소드 시작 시 초기화
        transform.position = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        rb.linearVelocity = Vector2.zero;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. 플레이어 관련 관측
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        sensor.AddObservation(directionToPlayer); // 방향 (2)
        sensor.AddObservation(distanceToPlayer / 10.0f); // 거리 정규화 (1)
        
        // 2. 장애물 탐지를 위한 레이캐스트
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
        for (int i = 0; i < rayAngles.Length; i++)
        {
            Vector2 rayDirection = Quaternion.Euler(0, 0, rayAngles[i]) * Vector2.right;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayDistance, obstacleLayer);
            
            float hitDistance = hit.collider != null ? hit.distance / rayDistance : 1.0f;
            sensor.AddObservation(hitDistance); // 각 방향의 장애물 거리 (8)
        }
        
        // 3. 다른 적들의 위치 정보
        foreach (EnemyAgent enemy in otherEnemies)
        {
            if (enemy != null)
            {
                Vector2 dirToEnemy = (enemy.transform.position - transform.position).normalized;
                float distToEnemy = Vector2.Distance(transform.position, enemy.transform.position) / 10.0f;
                sensor.AddObservation(dirToEnemy); // 방향 (2)
                sensor.AddObservation(distToEnemy); // 거리 (1)
            }
            else
            {
                sensor.AddObservation(Vector2.zero); // (2)
                sensor.AddObservation(1.0f); // (1)
            }
        }
        
        // 4. 자신의 속도
        sensor.AddObservation(rb.linearVelocity.normalized); // (2)
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // 이동 행동
        float moveX = actionBuffers.ContinuousActions[0];
        float moveY = actionBuffers.ContinuousActions[1];
        
        Vector2 movement = new Vector2(moveX, moveY).normalized * moveSpeed;
        rb.linearVelocity = movement;
        
        // 보상 계산
        CalculateReward();
    }
    
    private void CalculateReward()
    {
        // 1. 플레이어와의 거리 보상 (더 가까워질수록 보상 증가)
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float distanceReward = Mathf.Exp(-0.1f * distanceToPlayer) * 2.0f;
        AddReward(distanceReward);
        
        // 2. 플레이어에게 도착하면 큰 보상 및 에피소드 종료
        if (distanceToPlayer < 1.0f)
        {
            AddReward(10.0f);
            EndEpisode();
        }
        
        // 3. 장애물 충돌 시 패널티
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, 0.5f, obstacleLayer);
        if (obstacles.Length > 0)
        {
            AddReward(-1.0f);
        }
        
        // 4. 포위 보너스
        float surroundBonus = CalculateSurroundBonus();
        AddReward(surroundBonus * 0.5f);
    }
    
    private float CalculateSurroundBonus()
    {
        float bonus = 0f;
        Vector2 myDirToPlayer = (playerTransform.position - transform.position).normalized;
        
        foreach (EnemyAgent enemy in otherEnemies)
        {
            if (enemy != null)
            {
                Vector2 enemyDirToPlayer = (playerTransform.position - enemy.transform.position).normalized;
                float dotProduct = Vector2.Dot(myDirToPlayer, enemyDirToPlayer);
                
                if (dotProduct < 0.2f) // 적들이 다른 각도로 포위하고 있으면 보상 증가
                {
                    bonus += 0.5f;
                }
            }
        }
        
        return bonus;
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 디버깅을 위한 휴리스틱 컨트롤 (수동 테스트용)
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
}
