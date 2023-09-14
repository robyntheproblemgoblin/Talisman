using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class MeleeBT : EnemyBT
{
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new StatueMode(this),
            new Sequence(new List<Node>
            {
                new CanSeePlayer(transform, m_fovRange),
                new TaskGoToTarget(transform, m_attackRange, m_speed),
                
                new Decorator(new List<Node>
                {
                   new Sequence(new List<Node>
                   {
                      new CanSeePlayer(transform, m_fovRange),
                      new TaskGoToTarget(transform, m_attackRange, m_speed),
                      new TaskAttack(this),
                      new CheckTargetInMeleeRange(transform, this,m_attackRange),
                   })
                })
            })
        });
        return root;
    }
}