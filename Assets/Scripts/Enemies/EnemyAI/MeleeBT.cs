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
                new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                    {
                        new CheckTargetInMeleeRange(transform, m_attackRange),
                        new TaskAttack(this, m_animator),                        
                        new TaskGoToTarget(this, transform, m_attackRange, m_speed),
                    }),
                    new TaskGoToTarget(this, transform, m_attackRange, m_speed),
                })

            }),
        });
        return root;
    }
}