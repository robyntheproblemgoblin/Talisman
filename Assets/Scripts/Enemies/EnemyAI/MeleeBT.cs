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
                new CheckTargetInFOVRange(transform, m_fovRange),
                new Selector(new List<Node>
                {
                   new TaskGoToTarget(transform, m_attackRange, m_speed),
                   new Sequence(new List<Node>
                   {
                      new CheckTargetInMeleeRange(transform, this,m_attackRange),
                      new TaskAttack(this),
                   })
                })
            })
        });
        return root;
    }
}