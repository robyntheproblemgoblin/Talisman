using BehaviourTree;
using UnityEngine;

public class StatueMode : Node
{    
    EnemyBT m_enemyBT;

    public StatueMode(EnemyBT enemyBT)
    {        
        m_enemyBT = enemyBT;
    }
    public override NodeState Evaluate()
    {
        if (m_enemyBT.m_isStatue)
        {
            m_state = NodeState.SUCCESS;
        }
        else
        {
            m_state = NodeState.FAILURE;
        }
        return m_state;
    }
}