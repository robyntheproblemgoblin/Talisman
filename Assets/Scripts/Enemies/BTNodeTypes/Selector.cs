using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in m_children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        m_state = NodeState.SUCCESS;
                        return m_state;
                    case NodeState.RUNNING:
                        m_state = NodeState.RUNNING;
                        return m_state;
                    default:
                        continue;
                }
            }
            m_state=NodeState.FAILURE;
            return m_state;
        }
    }
}
