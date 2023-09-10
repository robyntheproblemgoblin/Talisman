using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class MeleeBT : EnemyBT
{
    new void Start()
    {
        m_currentHP = m_startingHP;
        m_animator = transform.gameObject.GetComponent<Animator>();
        m_animator.applyRootMotion = true;
        m_agent = transform.gameObject.GetComponent<NavMeshAgent>();
        if (m_waypoints.Length > 0)
        {
            m_agent.SetDestination(m_waypoints[0].position);
        }
        m_agent.updatePosition = false;
        m_agent.updateRotation = true;
        m_root = SetupTree();
        m_playerFlameLayer = (int)Mathf.Log(LayerMask.GetMask("Flame"), 2);
        m_playerSwordLayer = (int)Mathf.Log(LayerMask.GetMask("Sword"), 2);
        m_playerController = FindObjectOfType<PlayerController>();
    }   

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new StatueMode(transform, this),
             new Sequence(new List<Node>
            {
                new CheckTargetInFOVRange(transform),
                new TaskGoToTarget(transform),
            }),
            new Sequence(new List<Node>
            {
                new CheckTargetInAttackRange(transform),
//                new TaskGoToTarget(transform),
            })
        });        
        return root;
    }
}


