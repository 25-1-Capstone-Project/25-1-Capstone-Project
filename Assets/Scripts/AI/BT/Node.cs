using System.Collections.Generic;
using System;
using UnityEngine;

public interface INode
{
    public enum State { Run, Success, Failed }

    public INode.State Evaluate();  //상태 판단 후 리턴
}

public class ActionNode : INode
{
    public Func<INode.State> action;

    public ActionNode(Func<INode.State> action)
    {
        this.action = action;
    }

    public INode.State Evaluate()
    {
        return action?.Invoke() ?? INode.State.Failed;
    }
}

public class SelectorNode : INode
{
    List<INode> children;
    public SelectorNode() { children = new List<INode>(); }
    public void Add(INode node) { children.Add(node); }
    public INode.State Evaluate()
    {
        foreach (INode child in children)
        {
            INode.State state = child.Evaluate();

            switch (state)
            {
                case INode.State.Success:
                    return INode.State.Success;
                case INode.State.Run:
                    return INode.State.Run;
            }
        }
        return INode.State.Failed;
    }
}

public class SequenceNode : INode
{
    List<INode> children;
    public SequenceNode() { children = new List<INode>(); }
    public void Add(INode node) { children.Add(node); }
    public INode.State Evaluate()
    {
        if (children.Count <= 0)
            return INode.State.Failed;
        foreach (INode child in children)
        {
            switch (child.Evaluate())
            {
                case INode.State.Run:
                    return INode.State.Run;
                case INode.State.Success:
                    continue;
                case INode.State.Failed:
                    return INode.State.Failed;
            }
        }
        return INode.State.Success;
    }
}