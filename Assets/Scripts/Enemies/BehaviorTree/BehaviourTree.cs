using UnityEngine;

namespace BehaviourTree
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        Node m_root = null;

        void Start()
        {
            m_root = SetupTree();
        }

        void  Update()
        {
            if(m_root != null)
            {
                m_root.Evaluate();
            }
        }

        protected abstract Node SetupTree();
    }
}
