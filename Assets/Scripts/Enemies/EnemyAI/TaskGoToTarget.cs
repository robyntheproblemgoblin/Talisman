using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    Transform m_transform;
    Animator m_animator;
    NavMeshAgent m_agent;
    EnemyBT m_enemy;
    float m_attackRange;
    float m_speed;
    public TaskGoToTarget(EnemyBT enemy, Transform transform, float attackRange, float speed)
    {
        m_transform = transform;
        m_animator = transform.GetComponent<Animator>();
        m_agent = transform.GetComponent<NavMeshAgent>();
        m_enemy = enemy;
        m_attackRange = attackRange;
    }
    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        
        float distance = Vector3.Distance(m_transform.position, target.position + Vector3.down);
        if (distance >= m_attackRange && m_enemy.m_canAttack)
        {
            m_transform.position = Vector3.MoveTowards(
                m_transform.position,
                EnemyBuffer(target.position + Vector3.down),
                m_speed * Time.deltaTime);
            m_transform.LookAt(target.position + Vector3.down);        
            m_state = NodeState.SUCCESS;        
            return m_state;            
        }                
        else
        {
            m_state = NodeState.RUNNING;
            return m_state;
        }

    }
    Vector3 EnemyBuffer(Vector3 pos)
    {
        Vector3 direction = (m_transform.position - pos).normalized;
        return pos - (direction * m_attackRange);
    }
}
