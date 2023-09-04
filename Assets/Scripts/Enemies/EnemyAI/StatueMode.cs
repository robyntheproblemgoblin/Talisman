using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueMode : Node
{
    Transform m_transform;
    Animator m_animator;
    EnemyBT m_enemyBT;

    public StatueMode(Transform transform, EnemyBT enemyBT)
    {
        m_transform = transform;
        m_animator = transform.GetComponent<Animator>();
        m_enemyBT = enemyBT;
    }
    public override NodeState Evaluate()
    {
        if (m_enemyBT.m_isStatue)
        {
            m_state = NodeState.RUNNING;            
        }
        else
        {
            m_state = NodeState.FAILURE;
        }
            return m_state;
    }   
}