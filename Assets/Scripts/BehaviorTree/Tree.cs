using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{

    public abstract class Tree : MonoBehaviour
    {
        Node m_root = null;

        void Start()
        {
            m_root = SetupTree();
        }

        void Update()
        {
            if(m_root != null)
            {
                m_root.Evaluate();
            }
        }

        protected abstract Node SetupTree();
    }
}
