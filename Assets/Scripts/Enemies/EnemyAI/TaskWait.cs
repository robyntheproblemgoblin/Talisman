using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskWait : Node
{
    Transform m_transform;
    Animator m_animator;
    NavMeshAgent m_agent;    
    float m_current;
    float m_waitTime;
    EnemyBT m_enemy;
    public TaskWait(EnemyBT enemy, Transform transform, float waitTime)
    {
        m_transform = transform;
        m_animator = transform.GetComponent<Animator>();
        m_agent = transform.GetComponent<NavMeshAgent>();        
        m_waitTime = waitTime;
    }
    public override NodeState Evaluate()
    {
        Debug.Log(m_current);
        if (m_current <= 0f)
        {
            m_agent.SetDestination(m_transform.position);
            m_current += Time.deltaTime;
            m_state = NodeState.RUNNING;
            return m_state;
        }
        else if (m_current < m_waitTime)
        {
            m_current += Time.deltaTime;
            m_state = NodeState.RUNNING;
            return m_state;
        }
        else
        {            
            m_current = 0f;
            return NodeState.SUCCESS;
        }
    }

}
