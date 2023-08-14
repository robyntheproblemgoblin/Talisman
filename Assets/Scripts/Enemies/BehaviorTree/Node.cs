using System.Collections.Generic;

namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING, 
        SUCCESS, 
        FAILURE
    }

    public class Node
    {
        protected NodeState m_state;

        public Node m_parent;
        protected List<Node> m_children = new List<Node>();
        Dictionary<string, object> m_dataContext = new Dictionary<string, object>();

        public Node()
        {
            m_parent = null;
        }

        public Node(List<Node> children)
        {
            foreach(Node child in children)
            {
                Attach(child);
            }    
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        void Attach(Node node)
        {
            node.m_parent = this;
            m_children.Add(node);
        }

        public void SetData(string key, object value)
        {
            m_dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object val = null;
            if (m_dataContext.TryGetValue(key, out val))
            {
                return val;
            }

            Node node = m_parent;
            if(node != null)
            {
                val = node.GetData(key);
            }
            return val;
        }

        public bool ClearData(string key)
        {
            bool cleared = false;
            if(m_dataContext.ContainsKey(key))
            {
                m_dataContext.Remove(key);
                return true;
            }

            Node node = m_parent;
            if(node != null)
            {
                cleared = node.ClearData(key);
            }
            return cleared;
        }
    }
}