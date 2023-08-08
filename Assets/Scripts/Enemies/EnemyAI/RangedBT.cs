using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class RangedBT : EnemyBT
{
    protected override Node SetupTree() 
    {        
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckTargetInFOVRange(transform),
                new TaskGoToTarget(transform),
            }),
            new TaskPatrol(transform, m_waypoints, m_agent),
        });


        return root;
    }
}
