using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class EnemyAgent : Agent
{
    public Transform player; // 플레이어 위치
    private Rigidbody rb;

    public float moveSpeed = 3f; // 이동 속도

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // 적 위치 초기화
        transform.position = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        rb.linearVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 플레이어와의 상대적 위치
        Vector3 relativePosition = player.position - transform.position;
        sensor.AddObservation(relativePosition.normalized);

        // 현재 속도
        sensor.AddObservation(rb.linearVelocity.normalized);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 이동 처리
        Vector3 move = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        rb.AddForce(move * moveSpeed, ForceMode.VelocityChange);
    }

    // 플레이어와 접촉했을 때 보상 부여
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AddReward(1.0f); // 플레이어를 공격했으므로 보상 부여
            EndEpisode(); // 에피소드 종료
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }
}
