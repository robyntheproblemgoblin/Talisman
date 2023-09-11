using BehaviourTree;
using UnityEngine;

public class TaskGoToTarget : Node
{
    Transform m_transform;
    Animator m_animator;
    float m_attackRange;
    float m_speed;
    public TaskGoToTarget(Transform transform, float attackRange, float speed)
    {
        m_transform = transform;
        m_animator = transform.GetComponent<Animator>();
        m_attackRange = attackRange;
    }
    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        float distance = Vector3.Distance(m_transform.position, target.position + Vector3.down);
        if (distance > m_attackRange)
        {
            m_transform.position = Vector3.MoveTowards(
                m_transform.position,
                EnemyBuffer(target.position + Vector3.down),
                m_speed * Time.deltaTime);
            m_transform.LookAt(target.position + Vector3.down);        
            m_state = NodeState.RUNNING;        
            return m_state;            
        }                
        else
        {
            m_state = NodeState.FAILURE;
            return m_state;
        }

    }
    Vector3 EnemyBuffer(Vector3 pos)
    {
        Vector3 direction = (m_transform.position - pos).normalized;
        return pos - (direction * m_attackRange);
    }
}
