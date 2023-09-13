using UnityEngine;
using BehaviourTree;

public class TaskAttack : Node
{
    EnemyBT m_enemy;

    public TaskAttack(EnemyBT enemy)
    {
        m_enemy = enemy;   
    }

    public override NodeState Evaluate()
    {    
        if(!m_enemy.m_canAttack)
        {
            m_state = NodeState.RUNNING;
            return m_state;
        }  
        return m_state;
    
    }
}
