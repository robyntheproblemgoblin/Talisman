using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckTargetInFOVRange : Node
{
    Transform m_transform;

    static int m_playerLayerMask = 1 << 6;
    Transform targetPos;
    NavMeshAgent m_agent;

    public CheckTargetInFOVRange(Transform transform)
    {
        m_transform = transform;
        m_agent = transform.GetComponent<NavMeshAgent>();
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(m_transform.position, EnemyBT.m_fovRange, m_playerLayerMask);

            if(colliders.Length > 0)
            {
                m_parent.m_parent.SetData("target", colliders[0].transform);
                targetPos = colliders[0].transform;
                m_agent.SetDestination(targetPos.position);
                m_state = NodeState.SUCCESS;
                return m_state;
            }
            m_state = NodeState.FAILURE;
            return m_state;
        }
        m_agent.SetDestination(targetPos.position);
        m_state = NodeState.SUCCESS;
        return m_state;
    }
}
