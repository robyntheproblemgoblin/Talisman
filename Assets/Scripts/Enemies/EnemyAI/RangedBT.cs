using System.Collections.Generic;
using BehaviourTree;

public class RangedBT : EnemyBT
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
            }),           
        });
        return root;
    }
}
