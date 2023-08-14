using BehaviourTree;
using UnityEngine;

public class TaskGoToTarget : Node
{
    Transform m_transform;
    Animator m_animator;
    public TaskGoToTarget(Transform transform)
    {
        m_transform = transform;
        m_animator = transform.GetComponent<Animator>();
    }
    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if(Vector3.Distance(m_transform.position, target.position + Vector3.down) > EnemyBT.m_attackRange)
        {
            m_transform.position = Vector3.MoveTowards(
                m_transform.position, 
                EnemyBuffer(target.position + Vector3.down),
                EnemyBT.m_speed * Time.deltaTime);
            m_transform.LookAt(target.position + Vector3.down);            
        }

        m_state = NodeState.RUNNING;
        return m_state;
    }
    Vector3 EnemyBuffer(Vector3 pos)
    {
        Vector3 direction = (m_transform.position - pos).normalized;
        return pos + (direction * EnemyBT.m_attackRange * 5);
    }
}
