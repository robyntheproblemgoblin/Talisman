using System.Collections.Generic;
using BehaviourTree;

public class RangedBT : EnemyBT
{
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
        });
        return root;
    }
}
