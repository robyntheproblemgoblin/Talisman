using BehaviourTree;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Decorator : Node
    {
        public Decorator() : base() { }
        public Decorator(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in m_children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        m_state = NodeState.FAILURE;
                        return m_state;
                    default:
                        continue;
                }
            }
            m_state = NodeState.RUNNING;
            return m_state;
        }
    }
}
