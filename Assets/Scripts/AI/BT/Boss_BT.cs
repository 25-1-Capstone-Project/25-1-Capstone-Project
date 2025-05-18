using UnityEngine;
using System.Collections.Generic;

public class Boss_BT : MonoBehaviour
{
    private Node _root;

    public float distanceToPlayer;
    public float currentHP;

    private Fuzzy _fuzzy;

    private void Start()
    {
        _fuzzy = new Fuzzy();

        // 도망 행동 시퀀스
        Node fleeSeq = new Sequence(new List<Node>
        {
            new IsHealthLowNode(() => currentHP < 30f),
            new IsPlayerFarNode(() => distanceToPlayer > 7f),
            new FleeNode()
        });

        // 공격 행동 시퀀스
        Node attackSeq = new Sequence(new List<Node>
        {
            new IsPlayerVisibleNode(() => distanceToPlayer < 10f),
            new FuzzySkillNode(_fuzzy, () => distanceToPlayer, () => currentHP)
        });

        _root = new Selector(new List<Node>
        {
            fleeSeq,
            attackSeq,
            new PatrolNode()
        });
    }

    private void Update()
    {
        _root.Evaluate();

        // 예시로 플레이어와 보스 간 거리, HP를 테스트용으로 갱신할 수 있음
        // distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // currentHP = bossHealth.CurrentHP;
    }
}
