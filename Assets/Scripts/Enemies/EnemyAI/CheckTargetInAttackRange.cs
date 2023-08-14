using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckTargetInAttackRange : Node
{
    Transform m_transform;
    Animator m_animator;
    float time = 1f;
    bool canAttack = true;

    public CheckTargetInAttackRange(Transform transform)
    {
        m_transform = transform;
        m_animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if(t == null)
        {
            m_state = NodeState.FAILURE;
            return m_state;
        }

        Transform target = (Transform)t;
        if (Vector3.Distance(m_transform.position, target.position) <= EnemyBT.m_attackRange && canAttack)
        {
            canAttack = false;
            m_animator.SetBool("AttackB", true);
            time = 3f;
            m_state = NodeState.SUCCESS;
            return m_state;
        }
        else if(!canAttack)
        {
            time -= Time.deltaTime;
            if(time <= 0)
            {
                canAttack = true;
            }  
            else if(time <= 2.5f)
            {
                m_animator.SetBool("AttackB", false);
            }
            m_state = NodeState.FAILURE;
            return m_state;
        }
       
        m_state = NodeState.FAILURE;
        return m_state;
    }
}
