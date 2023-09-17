using UnityEngine;
using BehaviourTree;
using UnityEngine.Rendering;

public class CheckTargetInMeleeRange : Node
{
    Transform m_transform;        
    float m_attackRange;

    public CheckTargetInMeleeRange(Transform transform, float attackRange)
    {
        m_transform = transform;        
        m_attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            m_state = NodeState.FAILURE;        
            return m_state;
        }        

        Transform target = (Transform)t;
        if (Vector3.Distance(m_transform.position, target.position) <= m_attackRange)
        {            
            m_state = NodeState.SUCCESS;        
            return m_state;
        }
        
        m_state = NodeState.FAILURE;
        return m_state;
    }
}
