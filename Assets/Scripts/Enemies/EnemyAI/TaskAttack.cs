using UnityEngine;
using BehaviourTree;

public class TaskAttack : Node
{
    Transform m_lastTarget;
    Enemy m_enemy;
    float m_attackTime = 1f;
    float m_attackCounter = 0f;
    //Animator m_animator;    

    public TaskAttack(Transform transform)
    {
        //m_animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (target != m_lastTarget)
        {
            m_enemy = target.GetComponent<Enemy>();
            m_lastTarget = target;
        }

        m_attackCounter += Time.deltaTime;
        if (m_attackCounter >= m_attackTime)
        {
            bool playerIsDead = m_enemy.TakeHit();
            Debug.Log("Attack");
            if (playerIsDead)
            {
                ClearData("target");
                // m_animator.SetBool("Attacking", false);
                // m_animator.SetBool("Won", true);
            }
            else
            {
                m_attackCounter = 0f;
            }
        }

        m_state = NodeState.RUNNING;
        return m_state;
    }
}
