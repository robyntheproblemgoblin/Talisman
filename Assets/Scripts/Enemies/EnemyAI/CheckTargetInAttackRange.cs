using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckTargetInAttackRange : Node
{
    Transform m_transform;
    //Animator m_animator;

    public CheckTargetInAttackRange(Transform transform)
    {
        m_transform = transform;
       // m_animator = transform.GetComponent<Animator>();
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
        if(Vector3.Distance(m_transform.position, target.position) <= EnemyBT.m_attackRange)
        {
           // m_animator.SetBool("Attacking", true);
           // m_animator.SetBool("Walking", false);


            m_state = NodeState.SUCCESS;
            return m_state;
        }
       
        m_state = NodeState.FAILURE;
        return m_state;
    }
}
