using System.Collections.Generic;
using System;
using UnityEngine;

public enum NodeState { Running, Success, Failure }


public abstract class Node
{
    protected NodeState _state;
    public NodeState State => _state;
    public abstract NodeState Evaluate();
}

// 기본 노드
public class Sequence : Node
{
    private List<Node> _nodes;
    public Sequence(List<Node> nodes) { _nodes = nodes; }

    public override NodeState Evaluate()
    {
        foreach (var node in _nodes)
        {
            var result = node.Evaluate();
            if (result == NodeState.Failure)
            {
                _state = NodeState.Failure;
                return _state;
            }
            if (result == NodeState.Running)
            {
                _state = NodeState.Running;
                return _state;
            }
        }
        _state = NodeState.Success;
        return _state;
    }
}

public class Selector : Node
{
    private List<Node> _nodes;
    public Selector(List<Node> nodes) { _nodes = nodes; }

    public override NodeState Evaluate()
    {
        foreach (var node in _nodes)
        {
            var result = node.Evaluate();
            if (result == NodeState.Success)
            {
                _state = NodeState.Success;
                return _state;
            }
            if (result == NodeState.Running)
            {
                _state = NodeState.Running;
                return _state;
            }
        }
        _state = NodeState.Failure;
        return _state;
    }
}

//조건 노드
public class IsHealthLowNode : Node
{
    private Func<bool> _check;
    public IsHealthLowNode(Func<bool> check) { _check = check; }
    public override NodeState Evaluate() => _check() ? NodeState.Success : NodeState.Failure;
}

public class IsPlayerFarNode : Node
{
    private Func<bool> _check;
    public IsPlayerFarNode(Func<bool> check) { _check = check; }
    public override NodeState Evaluate() => _check() ? NodeState.Success : NodeState.Failure;
}

public class IsPlayerVisibleNode : Node
{
    private Func<bool> _check;
    public IsPlayerVisibleNode(Func<bool> check) { _check = check; }
    public override NodeState Evaluate() => _check() ? NodeState.Success : NodeState.Failure;
}

//행동 노드
public class FleeNode : Node
{
    public override NodeState Evaluate()
    {
        Debug.Log("도망 행동 수행");
        // 실제 도망 로직 추가 가능
        return NodeState.Success;
    }
}

public class PatrolNode : Node
{
    public override NodeState Evaluate()
    {
        Debug.Log("순찰 행동 수행");
        // 실제 순찰 로직 추가 가능
        return NodeState.Success;
    }
}

public class FuzzySkillNode : Node
{
    private Fuzzy _fuzzy;
    private Func<float> _getDistance;
    private Func<float> _getHP;

    public FuzzySkillNode(Fuzzy fuzzy, Func<float> getDistance, Func<float> getHP)
    {
        _fuzzy = fuzzy;
        _getDistance = getDistance;
        _getHP = getHP;
    }

    public override NodeState Evaluate()
    {
        float dist = _getDistance();
        float hp = _getHP();
        SkillAction skill = _fuzzy.DecideSkillFuzzy(dist, hp);
        Debug.Log($"퍼지 스킬 결정: {skill}");
        // 실제 스킬 실행 코드 추가
        return NodeState.Success;
    }
}

public class FuzzySkillSelector : Node
{
    private readonly Fuzzy fuzzy;
    private readonly Func<float> getDistance;
    private readonly Func<float> getHealth;
    private readonly Action<SkillAction> executeSkill;

    public FuzzySkillSelector(Fuzzy fuzzy, Func<float> getDistance, Func<float> getHealth, Action<SkillAction> executeSkill)
    {
        this.fuzzy = fuzzy;
        this.getDistance = getDistance;
        this.getHealth = getHealth;
        this.executeSkill = executeSkill;
    }

    public override NodeState Evaluate()
    {
        float distance = getDistance();
        float hp = getHealth();
        SkillAction selectedSkill = fuzzy.DecideSkillFuzzy(distance, hp);
        executeSkill(selectedSkill);
        return NodeState.Success;
    }
}




