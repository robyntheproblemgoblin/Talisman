using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskAttack : Node
{
    EnemyBT m_enemy;
    NavMeshAgent m_agent;
    Animator m_animator;

    public TaskAttack(EnemyBT enemy, Animator anim)
    {
        m_enemy = enemy;
        m_agent = m_enemy.gameObject.GetComponent<NavMeshAgent>();
        m_animator = anim;
    }

    public override NodeState Evaluate()
    {
        if(m_enemy.m_canAttack)
        {
            m_animator.SetTrigger("Attack");
            m_agent.isStopped = true;
            m_state = NodeState.SUCCESS;
            return m_state;
        }        
     
        m_state = NodeState.FAILURE;
        return m_state;
    
    }
}
