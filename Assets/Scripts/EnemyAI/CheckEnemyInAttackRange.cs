using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckEnemyInAttackRange : Node
{
    private Transform m_transform;
    //private Animator m_animator;

    public CheckEnemyInAttackRange(Transform transform)
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
        if(Vector3.Distance(m_transform.position, target.position) < = EnemyBT.m_attackRange)
        {

        }
    }
}
