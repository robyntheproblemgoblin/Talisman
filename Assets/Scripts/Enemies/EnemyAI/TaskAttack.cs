using UnityEngine;
using BehaviourTree;

public class TaskAttack : Node
{    
    PlayerController m_player;
    float m_attackTime = 1f;
    float m_attackCounter = 0f;    

    public TaskAttack(Transform transform)
    {        
        m_player = MonoBehaviour.FindObjectOfType<PlayerController>();
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            m_state = NodeState.FAILURE;
        Debug.Log("TaskAttack is " + m_state);
            return m_state;
        }

        m_attackCounter += Time.deltaTime;
        if (m_attackCounter >= m_attackTime)
        {
            bool playerIsDead = m_player.m_health <= 0;
            Debug.Log("Attack");
            if (playerIsDead)
            {
                ClearData("target");                
            }
            else
            {
                m_attackCounter = 0f;
            }
        }

        m_state = NodeState.RUNNING;
        Debug.Log("TaskAttack is " + m_state);
        return m_state;
    }
}
